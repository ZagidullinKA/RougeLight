using log4net;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Mobs : Character
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Mobs));
    // Префаб для создания файла с характеристиками
    [SerializeField] private MobStats mobStatsPrefab;
    // Переменная для характеристик, через нее можно обращаться к базову stats
    public MobStats mobStats => stats as MobStats; 



    private Rigidbody2D rb;
    private DropManager dropManagerScript;

    private Transform player; // Ссылка на игрока

    

    protected override void Awake()
    {
        // Создаём новый экземпляр MobStats
        stats = Instantiate(mobStatsPrefab);

        if (mobStats != null)
        {
            log.Debug($"Моб инициализирован с ID: {mobStats.IdMob} и наградой за смерть: {mobStats.DeathPrice}");
        }

        InitializeCharacteristics(EnemyTypeCode.UnitTest);
        base.Awake();
        mobStats.IsEnemy = true; // Моб является врагом

        dropManagerScript = GetComponent<DropManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player != null)
        {
            RotateTowardsPlayer();
            base.Shoot();
        }

        base.HandlingAppliedDoTEffects();
    }

    void RotateTowardsPlayer()
    {
        // Вычисляем направление к игроку
        Vector2 direction = player.position - transform.position;
        direction.Normalize();

        // Вычисляем угол поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Создаем целевой поворот
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Плавно поворачиваем объект
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, mobStats.RotateSpeed * Time.deltaTime);
    }

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
        if (characterCode == CharacterStatCode.MoveSpeed)
        {
            EnemyTestAI mobsMoveScript = gameObject.GetComponent<EnemyTestAI>();
            mobsMoveScript.MoveSpeed = mobStats.MoveSpeed;
            log.Debug($"DropRadius обновлён: {mobStats.MoveSpeed}");
        }
    }


    // Инициализация характеристик
    private void InitializeCharacteristics(EnemyTypeCode enemyType)
    {
        var enemyData = EnemyTypesDictionary.GetCharacteristic(enemyType);
        float mobsMultiplier = GameGeneration.MobsMultipier;

        if (enemyData != null && mobStats != null)
        {
            // Получаем все наименнования переменных из ItemEnemyTypesDictionary и соответствующие условиям public и принадлежащие объекту, т.е. не статические
            var enemyFields = typeof(ItemEnemyTypesDictionary).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var field in enemyFields)
            {
                if (field.PropertyType == typeof(int) || field.PropertyType == typeof(float))
                {
                    string fieldName = field.Name; // Получаем имя переменной
                    string statName = char.ToLower(fieldName[0]) + fieldName.Substring(1); // меняем регистр 1 буквы имени на нижний

                    // Получаем соответствующее имя переменной из класса Mob 
                    var mobField = typeof(MobStats).GetField(statName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (mobField != null)
                    {
                        // Конвертируем значение во float
                        var value = Convert.ToSingle(field.GetValue(enemyData));
                        float? newValue = fieldName == "DeathPrice" ? value : (float)Math.Round(value * mobsMultiplier); // Получаем значение, которое будем присваивать
                        mobStats.SetStat(fieldName, newValue); // Присваеваем переменной новое значение
                    }
                }
            }

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


    protected override void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        log.Debug("TakeDamage. Die. idMob = " + mobStats.IdMob);
        base.TakeDamage(damage, typeDamage);
    }

    protected override void Die()
    {
        Destroy(GetComponent<BoxCollider2D>());
        if (mobStats.IsCanDie)
        {
            mobStats.IsCanDie = false;
            log.Debug("Die. idMob = " + mobStats.IdMob);
            dropManagerScript.DropLoss();
            Observer.IncrementCountKill(mobStats.DeathPrice);
            base.Die();
        }
        
    }
}
