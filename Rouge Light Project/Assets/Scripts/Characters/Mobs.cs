using log4net;
using System;
using System.Collections.Generic;
using UnityEngine;

// Класс Mobs - базовый класс для всех вражеских мобов
// Наследуется от Character и реализует интерфейс IAttacker (атакующего объекта)
public class Mobs : Character, IAttacker
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Mobs));
    // Префаб для создания файла с характеристиками
    [SerializeField] private MobStats mobStatsPrefab;
    // Переменная для характеристик, через нее можно обращаться к базову stats
    public MobStats mobStats => stats as MobStats; // Приведение базовых stats к MobStats

    // Физическое тело моба
    private Rigidbody2D rb;
    // Менеджер дропа (предметов, выпадающих после смерти)
    private DropManager dropManagerScript;
    // Ссылка на трансформ игрока для преследования и атаки
    private Transform player;
    // Компонент стрельбы
    private Shooting shooting;

    // Метод инициализации, вызываемый при создании объекта
    protected override void Awake()
    {
        // Создаём новый экземпляр MobStats из префаба
        stats = Instantiate(mobStatsPrefab);

        if (mobStats != null)
        {
            log.Debug($"Моб инициализирован с ID: {mobStats.IdMob} и наградой за смерть: {mobStats.DeathPrice}");
        }

        // Инициализация характеристик моба (по умолчанию UnitTest тип)
        InitializeCharacteristics(EnemyTypeCode.UnitTest);
        base.Awake();
        mobStats.IsEnemy = true; // Моб является врагом

        // Получаем компоненты и ссылки
        dropManagerScript = GetComponent<DropManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        shooting = gameObject.GetComponent<Shooting>();
    }

    // Метод Update вызывается каждый кадр
    void Update()
    {
        if (player != null)
        {
            // Поворачиваем моба к игроку и стреляем
            RotateTowardsPlayer();
            Shoot();
        }

        // Обработка активных DoT-эффектов (Damage over Time)
        base.HandlingAppliedDoTEffects();
    }

    // Реализация IAttacker - метод стрельбы
    public void Shoot()
    {
        // Вызываем выстрел с текущими характеристиками моба
        shooting.Shot(mobStats.Dmg, mobStats.CritChance, mobStats.AtkSpeed, mobStats.BulletFlySpeed, mobStats.BulletTimeAlive, mobStats.GetUsableDots());
    }

    // Метод поворота моба в сторону игрока
    void RotateTowardsPlayer()
    {
        // Вычисляем направление к игроку
        Vector2 direction = player.position - transform.position;
        direction.Normalize();

        // Вычисляем угол поворота через арктангенс
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Создаем целевой поворот
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Плавно поворачиваем объект с учетом скорости поворота
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, mobStats.RotateSpeed * Time.deltaTime);
    }

    // Переопределенный метод установки характеристик
    public override void SetStat(string statName, float? value)
    {
        if (mobStats == null)
        {
            log.Debug("Stats не назначены!");
            return;
        }

        if (value == null)
        {
            log.Debug($"Значение для {statName} не указано!");
            return;
        }

        // Проверка существования statName в enum CharacterStatCode
        if (Enum.TryParse<CharacterStatCode>(statName, ignoreCase: true, out CharacterStatCode statCode))
        {
            mobStats.SetStat(statName, value);
            ApplySpecialEffects(statName);
        }
    }

    //Метод дополнитльеных изменений, помимо самой переменной
    protected override void ApplySpecialEffects(string statName)
    {
        CharacterStatCode characterCode = (CharacterStatCode)Enum.Parse(typeof(CharacterStatCode), statName);
        // Если изменилась скорость движения - обновляем соответствующий компонент
        if (characterCode == CharacterStatCode.MoveSpeed)
        {
            EnemyTestAI mobsMoveScript = gameObject.GetComponent<EnemyTestAI>();
            mobsMoveScript.MoveSpeed = mobStats.MoveSpeed;
            log.Debug($"DropRadius обновлён: {mobStats.MoveSpeed}");
        }
    }

    // Инициализация характеристик моба на основе типа врага
    private void InitializeCharacteristics(EnemyTypeCode enemyType)
    {
        // Получаем данные врага из словаря
        var enemyData = EnemyTypesDictionary.GetCharacteristic(enemyType);
        // Множитель сложности моба
        float mobsMultiplier = GameGeneration.MobsMultipier;

        if (enemyData != null && mobStats != null)
        {
            // Получаем все публичные нестатические свойства класса ItemEnemyTypesDictionary
            var enemyFields = typeof(ItemEnemyTypesDictionary).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            // Проходим по всем полям данных врага
            foreach (var field in enemyFields)
            {
                // Обрабатываем только числовые поля
                if (field.PropertyType == typeof(int) || field.PropertyType == typeof(float))
                {
                    string fieldName = field.Name;
                    // Преобразуем имя поля к camelCase (первая буква маленькая)
                    string statName = char.ToLower(fieldName[0]) + fieldName.Substring(1);

                    // Ищем соответствующее поле в MobStats
                    var mobField = typeof(MobStats).GetField(statName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (mobField != null)
                    {
                        // Получаем значение из данных врага
                        var value = Convert.ToSingle(field.GetValue(enemyData));
                        // Для DeathPrice не применяем множитель сложности
                        float? newValue = fieldName == "DeathPrice" ? value : (float)Math.Round(value * mobsMultiplier);
                        mobStats.SetStat(fieldName, newValue);
                    }
                }
            }

            // Добавляем случайные DoT-эффекты мобу
            for (int j = 0; j < enemyData.CountOfDots; j++)
            {
                ItemDot itemDot = DotsDictionary.GetRandomDot();
                mobStats.SetUsableDots((new UsableDotEffect(itemDot.Code, itemDot.BaseDotDmg, itemDot.BaseDotDuration)));
            }

            log.Debug($"InitializeCharacteristics. Характеристики моба инициализированы - DeathPrice: {mobStats.DeathPrice}");
        }
        else
        {
            if (enemyData == null)
            {
                log.Warn($"Данные врага для {enemyType} не найдены в EnemyTypesDictionary.");
            }
            if (mobStats == null)
            {
                log.Warn("mobStats не инициализирован.");
            }
        }
    }

    // Переопределенный метод получения урона
    protected override void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        log.Debug("TakeDamage. Die. idMob = " + mobStats.IdMob);
        base.TakeDamage(damage, typeDamage);
    }

    // Переопределенный метод смерти моба
    protected override void Die()
    {
        // Удаляем коллайдер, чтобы избежать дальнейших столкновений
        Destroy(GetComponent<BoxCollider2D>());
        if (mobStats.IsCanDie)
        {
            mobStats.IsCanDie = false;
            log.Debug("Die. idMob = " + mobStats.IdMob);
            // Вызываем дроп предметов
            dropManagerScript.DropLoss();
            // Увеличиваем счетчик убийств
            Observer.IncrementCountKill(mobStats.DeathPrice);
            base.Die();
        }
    }
}