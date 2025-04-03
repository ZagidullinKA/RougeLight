using UnityEngine;
using log4net;
using System;
using System.Collections.Generic;


// Класс Hero - основной класс управления игровым персонажем
// Наследуется от Character, реализует интерфейсы атакующего (IAttacker) и подвижного объекта (IMovable)
public class Hero : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Hero));
    // Префаб для создания файла с характеристиками
    [SerializeField] private HeroStats heroStatsPrefab;
    // Переменная для характеристик, через нее можно обращаться к базову stats 
    public HeroStats heroStats => stats as HeroStats; // Приведение базовых stats к HeroStats

    // Компоненты и параметры управления
    public Rigidbody2D rb;              // Физическое тело персонажа
    public Shooting shooting;           // Компонент стрельбы
    public Transform firePoint;         // Точка выстрела
    private Vector2 moveVector;         // Вектор движения
    private bool isShooting = false;    // Флаг стрельбы

    //Колайдер для подбора дропа
    private CircleCollider2D colliderDropRadius;
    // линия отражающая радиус подбора (Для тестов)
    private LineRenderer lineRendererDropRadius;

    // Множитель улучшения характеристик при повышении уровня
    private const float UpgradeLvlFactor = 1.5f;

    // Метод инициализации, вызываемый при создании объекта
    protected override void Awake()
    {
        // Создаем экземпляр характеристик из префаба
        stats = Instantiate(heroStatsPrefab);

        // Получаем компоненты
        rb = GetComponent<Rigidbody2D>();
        shooting = GetComponent<Shooting>();

        if (heroStats == null)
        {
            log.Error("heroStats не инициализирован");
        }

        heroStats.IsEnemy = false; // Герой не является врагом

        // Настройка коллайдера для подбора предметов
        colliderDropRadius = GetComponent<CircleCollider2D>();
        if (colliderDropRadius != null)
        {
            colliderDropRadius.isTrigger = true;
            // Создаем LineRenderer для визуализации радиуса подбора
            lineRendererDropRadius = gameObject.AddComponent<LineRenderer>();

            // Задаем для LineRenderer базовые настройки
            lineRendererDropRadius.startWidth = 0.02f;
            lineRendererDropRadius.endWidth = 0.02f;
            lineRendererDropRadius.useWorldSpace = false;
            lineRendererDropRadius.material = new Material(Shader.Find("Sprites/Default"));
            lineRendererDropRadius.startColor = new Color(0f, 1f, 0f, 0.1f);
            lineRendererDropRadius.endColor = new Color(0f, 1f, 0f, 0.1f);
        }
        else
        {
            log.Error("CircleCollider2D DropRadius is null");
        }

        // Инициализация характеристик и эффектов
        InitializeCharacteristicsAndDots();
        base.Awake();
        // Обновление UI здоровья
        UIManager.Instance.printActualHP(heroStats.ActualHP);

    }

    // Переопределенный метод получения урона
    protected override void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        base.TakeDamage(damage, typeDamage);
        // Обновление UI здоровья после получения урона
        UIManager.Instance.printActualHP(heroStats.ActualHP);

    }

    // Метод Update вызывается каждый кадр
    void Update()
    {
        // Проверка нажатия кнопки стрельбы
        if (Input.GetKey(KeyCode.Mouse0))
        {
            isShooting = true;
        }

        // Вызов обработчика дотов
        base.HandlingAppliedDoTEffects();
        // Обновление UI эффектов
        UIManager.Instance.printRecievedDots(heroStats.GetRecievedDots());
    }

    // Метод рисования круга для отображения радиуса дропа
    void DrawCircle()
    {
        int segments = 360;
        lineRendererDropRadius.positionCount = segments + 1;

        float angle = 0f;
        float angleStep = 360f / segments;

        // Рисуем круг из сегментов
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * colliderDropRadius.radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * colliderDropRadius.radius;

            lineRendererDropRadius.SetPosition(i, new Vector3(x, y, 0) + (Vector3)colliderDropRadius.offset);
            angle += angleStep;
        }
    }

    // Метод FixedUpdate вызывается с фиксированной частотой
    private void FixedUpdate()
    {
        Move();  // Обработка движения
        Shoot(); // Обработка стрельбы
    }

    // Обработчик столкновений с триггерами
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что объект можно подобрать
        if (other.CompareTag("Drop"))
        {
            log.Debug("Предмет подобрали! - " + other.name);
            Drop drop = other.GetComponent<Drop>();

            // Обработка разных типов дропа
            switch (drop.DropCode)
            {
                case TypeOfDrop.TYPE_CHARACTER:
                    SetStat(drop.ItemCode, drop.Update);
                    break;
                case TypeOfDrop.TYPE_DOT:
                    DotCode dotCode = (DotCode)Enum.Parse(typeof(DotCode), drop.ItemCode);
                    UpgradeUsableDots(dotCode, drop.Update, (bool)drop.IsDmgUpIfDot);
                    break;
                case TypeOfDrop.TYPE_MONEY:
                    Observer.increaseMoney(drop.Update);
                    break;
                case TypeOfDrop.TYPE_HEAL:
                    Heal(drop.Update);
                    break;
                default:
                    log.Error($"Неизвестная тип дропа: {drop.DropCode}");
                    return;
            }

            Destroy(other.gameObject);
            log.Debug("Дроп уничтожен");
        }
    }

    // Реализация IAttacker - метод стрельбы
    public void Shoot()
    {
        if (isShooting)
        {
            // Вызываем метод выстрела с текущими характеристиками
            shooting.Shot(heroStats.Dmg, heroStats.CritChance, heroStats.AtkSpeed, heroStats.BulletFlySpeed, heroStats.BulletTimeAlive, heroStats.GetUsableDots());
            isShooting = false;
        }
    }

    // Реализация IMovable - метод движения
    public void Move()
    {
        // Получаем ввод с клавиатуры
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");

        // Перемещаем персонажа с учетом скорости
        rb.MovePosition(rb.position + moveVector * heroStats.MoveSpeed * Time.deltaTime);
        RotateTowardsMouse(); // Поворачиваем персонажа к курсору
    }

    // Инициализация характеристик и эффектов
    private void InitializeCharacteristicsAndDots()
    {
        // Используем справочник всех характеристик
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            if (item.Upgradable)
            {
                // Получаем улучшаемые характеристики из справочника улучшаемых характеристик
                var upgradableItem = ImprovableCharactesDictionary.GetAllImprovableCharacteristicsAndDots().Find(x => x.Code == item.Code.ToString());
                if (upgradableItem is null)
                {
                    log.Error("upgradableItem is null - item.code = " + item.Code);
                    continue;
                }
                SetStat(item.Code.ToString(), upgradableItem.FinalValue);
            }
            else
            {
                // Используем базовые значения из справочника всех характеристик
                SetStat(item.Code.ToString(), item.BaseAmount);
            }
        }

        // Инициализация эффектов (DoT)
        foreach (var item in ImprovableCharactesDictionary.GetAllImprovableDots())
        {
            // Преобразование из string в DotCode
            DotCode dotCode = (DotCode)Enum.Parse(typeof(DotCode), item.Code);
            heroStats.SetUsableDots(new UsableDotEffect(dotCode, (int)item.FinalDotDmg, (int)item.FinalDotDur));
        }
        UIManager.Instance.printUsableDots(heroStats.GetUsableDots());
    }

    // Переопределенный метод установки характеристик
    public override void SetStat(string statName, float? value)
    {
        if (heroStats == null)
        {
            log.Debug("Stats не назначены!");
            return;
        }

        if (value == null)
        {
            log.Debug($"Значение для {statName} не указано!");
            return;
        }

        // Устанавливаем новое значение характеристики
        heroStats.SetStat(statName, value);
        ApplySpecialEffects(statName);

        if (UIManager.Instance == null)
        {
            log.Error("UIManager не найден!");
            return;
        }

        // Обновляем все характеристики в UI
        UIManager.Instance.printCharacters(
            heroStats.MaxHP,
            heroStats.Dmg,
            heroStats.AtkSpeed,
            heroStats.MoveSpeed,
            heroStats.Luck,
            heroStats.CritChance,
            heroStats.EvadeChance,
            heroStats.Armor,
            heroStats.DebuffResist,
            heroStats.Vampire,
            heroStats.HpFromDropRestore,
            heroStats.DropRadius,
            heroStats.BulletFlySpeed,
            heroStats.BulletTimeAlive
        );
    }

    //Метод дополнитльеных изменений, помимо самой переменной
    protected override void ApplySpecialEffects(string statName)
    {
        CharacterStatCode characterCode = (CharacterStatCode)Enum.Parse(typeof(CharacterStatCode), statName);

        // Если изменился радиус подбора - обновляем коллайдер и визуализацию
        if (characterCode == CharacterStatCode.DropRadius && colliderDropRadius != null)
        {
            colliderDropRadius.radius = heroStats.DropRadius;
            DrawCircle();
            log.Debug($"DropRadius обновлён: {heroStats.DropRadius}");
        }
    }

    // Добавление нового эффекта в список доступных
    public void AddDotInUsableDotsArrayOfCode(DotCode code)
    {
        var item = ImprovableCharactesDictionary.GetImprovableCharacteristicOrDot(code.ToString());

        if (item == null)
        {
            log.Error("AddDotInUsableDotsArrayOfCode. дот который пытаемся добавить не найден в справочнике, code = " + code);
            return;
        }

        // Преобразование из string в DotCode
        DotCode dotCode = (DotCode)Enum.Parse(typeof(DotCode), item.Code);
        heroStats.SetUsableDots(new UsableDotEffect(dotCode, (int)item.FinalDotDmg, (int)item.FinalDotDur));
    }

    // Улучшение параметров эффекта
    public void UpgradeUsableDots(DotCode code, float? value, bool typeUpgradeDmgDot)
    {
        bool seacrhDot = true; // Флаг поиска эффекта
        for (int i = 0; i < heroStats.GetUsableDots().Count; i++)
        {
            if (heroStats.GetUsableDots()[i].Code == code)
            {
                if (typeUpgradeDmgDot)
                {
                    // Улучшение урона эффекта
                    if (heroStats.GetUsableDots()[i].DotDur == 0)
                    {
                        heroStats.GetUsableDots()[i].DotDur = ImprovableCharactesDictionary.GetFinalDotDurOfCode(code.ToString());
                        log.Warn("DotDur = 0, code = " + code);
                    }
                    heroStats.GetUsableDots()[i].DotDmg += (int)value;
                    log.Debug("Улучшили dot " + heroStats.GetUsableDots()[i].Code + ", dmg на " + value + ", округлили до " + (int)value + ", теперь он = " + heroStats.GetUsableDots()[i].DotDmg);
                }
                else
                {
                    // Улучшение длительности эффекта
                    if (heroStats.GetUsableDots()[i].DotDmg == 0)
                    {
                        heroStats.GetUsableDots()[i].DotDmg = ImprovableCharactesDictionary.GetFinalDotDmgOfCode(code.ToString());
                        log.Warn("DotDmg = 0, code = " + code);
                    }
                    heroStats.GetUsableDots()[i].DotDur += (int)value;
                    log.Debug("Улучшили dot " + heroStats.GetUsableDots()[i].Code + ", dur на " + value + ", округлили до " + (int)value + ", теперь он = " + heroStats.GetUsableDots()[i].DotDur);
                }
                seacrhDot = false;
                break;
            }
        }

        // Если эффект не найден - добавляем новый
        if (seacrhDot)
        {
            log.Debug("Дот не был найден в списке usableDotsArray героя, code = " + code);
            AddDotInUsableDotsArrayOfCode(code);
        }

        UIManager.Instance.printUsableDots(heroStats.GetUsableDots());
    }

    // Повышение уровня персонажа
    public void IncrementLvl(int lvl)
    {
        if (lvl < 1)
        {
            log.Error("IncrementLvl. Передается " + lvl + " уровень");
        }

        log.Debug("IncrementLvl. Передается " + lvl + " уровень");

        // 1. Извлекаем общее вычисление в отдельную функцию
        float CalculateUpgrade(int level)
        {
            float result = level * UpgradeLvlFactor;
            return result > 1 ? (float)Math.Ceiling(result) : 1;
        }

        // 2. Используем Dictionary для группировки характеристик
        var upgrades = new Dictionary<CharacterStatCode, float>
        {
            [CharacterStatCode.Dmg] = CalculateUpgrade(lvl),
            [CharacterStatCode.AtkSpeed] = CalculateUpgrade(lvl),
            [CharacterStatCode.MaxHP] = CalculateUpgrade(lvl)
        };

        // 3. Применяем характеристики в цикле
        foreach (var characteristic in upgrades)
        {
            log.Debug("IncrementLvl. Увелечение характеристики : " + characteristic.Key + " на " + characteristic.Value);
            SetStat(characteristic.Key.ToString(), characteristic.Value);
        }
    }

    // Поворот персонажа в сторону курсора мыши
    void RotateTowardsMouse()
    {
        // Получаем позицию курсора в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Вычисляем направление от игрока к курсору
        Vector2 direction = mousePosition - transform.position;

        // Вычисляем угол для поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Устанавливаем новый угол поворота
        rb.rotation = angle;
    }

    // Переопределенный метод лечения
    public override void Heal(int amount)
    {
        base.Heal(amount);
        UIManager.Instance.printActualHP(heroStats.ActualHP);

    }

    // Переопределенный метод смерти
    protected override void Die()
    {
        if (!heroStats.IsGodMode)
        {
            if (heroStats.IsCanDie)
            {
                heroStats.IsCanDie = false;
                base.Die();
            }
        }
        else
        {
            log.Debug("Ты бы умер, но ты либо тестер, либо читер");
            heroStats.ActualHP = heroStats.MaxHP;
            UIManager.Instance.printActualHP(heroStats.ActualHP);
        }
    }

}

