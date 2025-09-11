using UnityEngine;
using log4net;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

//удалены неиспользуемые библиотеки

public class Hero : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Hero));
    // Префаб для создания файла с характеристиками
    [SerializeField] private HeroStats heroStatsPrefab;
    // Переменная для характеристик, через нее можно обращаться к базову stats 
    public HeroStats heroStats => stats as HeroStats;

    public Rigidbody2D rb;
    private Vector2 moveVector;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction shootAction;

    //Колайдер для подбора дропа
    private CircleCollider2D colliderDropRadius;
    // линия отражающая радиус подбора (Для тестов)
    private LineRenderer lineRendererDropRadius;

    // Переменная, которая отражает увелечение характеристик при получении уровня
    private const float UpgradeLvlFactor = 1.5f;

    

    protected override void Awake()
    {


        stats = Instantiate(heroStatsPrefab);

        rb = GetComponent<Rigidbody2D>();

        if (heroStats == null)
        {
            log.Error("heroStats не инициализирован");
        }

        heroStats.IsEnemy = false; // Герой не является врагом
        

        colliderDropRadius = GetComponent<CircleCollider2D>();
        if (colliderDropRadius != null)
        {
            colliderDropRadius.isTrigger = true;
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

        InitializeCharacteristicsAndDots();
        base.Awake();
        InitializeShootingModifier();
        UIManager.Instance.printActualHP(heroStats.ActualHP);




        // Проверяем, задан ли InputActions в инспекторе
        if (inputActions == null)
        {
            log.Error("InputActions не задан в инспекторе!");
        }

        // Ищем карту действий "Player" в InputActions
        var actionMap = inputActions.FindActionMap("Player");
        if (actionMap == null)
        {
            log.Error("Карта действий 'Player' не найдена!");
        }

        // Ищем действие "Shoot" в карте "Player"
        shootAction = actionMap.FindAction("Shoot");
        if (shootAction == null)
        {
            log.Error("Действие 'Shoot' не найдено в карте 'Player'!");
        }

        shootAction.started += ctx => OnShootStarted();         // Подписываемся на событие начала действия (нажатие ЛКМ)
        shootAction.Enable();                                   // Активируем действие для обработки ввода
    }

    

    private void OnShootStarted()
    {
        Shoot();
    }

    protected override void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        base.TakeDamage(damage, typeDamage);
        UIManager.Instance.printActualHP(heroStats.ActualHP);
    }

    void Update()
    {
        // Проверяем состояние кнопки через ReadValue
        if (shootAction.ReadValue<float>() > 0) // 1 = кнопка нажата
        {
            base.Shoot();
        }

        // Вызов обработчика дотов
        base.HandlingAppliedDoTEffects();
        UIManager.Instance.printRecievedDots(heroStats.GetRecievedDots());
    }

    // Метод рисования круга для отображения радиуса дропа
    void DrawCircle()
    {
        int segments = 360;
        lineRendererDropRadius.positionCount = segments + 1;

        float angle = 0f;
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * colliderDropRadius.radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * colliderDropRadius.radius;

            lineRendererDropRadius.SetPosition(i, new Vector3(x, y, 0) + (Vector3)colliderDropRadius.offset);
            angle += angleStep;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что объект можно подобрать
        if (other.CompareTag("Drop"))
        {
            log.Debug("Предмет подобрали! - " + other.name);
            Drop drop = other.GetComponent<Drop>();


            switch (drop.DropCode)
            {
                case TypeOfDrop.TYPE_CHARACTER:
                    SetStat(drop.ItemCode, drop.Update);
                    break;
                case TypeOfDrop.TYPE_DOT:
                    DotCode dotCode = (DotCode)Enum.Parse(typeof(DotCode), drop.ItemCode);
                    UpgradeUsableDots(dotCode, drop.Update, (bool) drop.IsDmgUpIfDot);
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

    // Реализация IMovable
    public void Move()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");

        rb.MovePosition(rb.position + moveVector * heroStats.MoveSpeed * Time.deltaTime);
        RotateTowardsMouse();
    }

    // Инициализация характеристик
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

        foreach (var item in ImprovableCharactesDictionary.GetAllImprovableDots())
        {
            // Преобразование из string в DotCode
            DotCode dotCode = (DotCode)Enum.Parse(typeof(DotCode), item.Code);
            heroStats.SetUsableDots(new UsableDotEffect(dotCode, (int)item.FinalDotDmg, (int)item.FinalDotDur));
        }
        UIManager.Instance.printUsableDots(heroStats.GetUsableDots());
    }

    private void InitializeShootingModifier()
    {
        // Получаем модификаторы уровня 1
        //heroStats.SetDefaultShotPoints();
        RedistributeShotPointsInRange(0, 360, 8, true);
        //RedistributeShotPointsInRange(270, 90, 8);

        // Получаем модификаторы уровня 2
        var level2Modifiers = ShootingModifiersDictionary.GetListModifiersByLevel(2);
        foreach (var modifier in level2Modifiers)
        {
            if (modifier.Code == TypeOfShootingModifier.Burst3)
            {
                heroStats.AddShootingModifierSecond(modifier.Code);
                log.Debug($"Добавлен модификатор второго этапа: {modifier.Code}");
                break; // Добавляем только один модификатор
            }
        }

        // Получаем модификаторы уровня 3
        var level3Modifiers = ShootingModifiersDictionary.GetListModifiersByLevel(3);
        foreach (var modifier in level3Modifiers)
        {
            if (modifier.Code == TypeOfShootingModifier.Buckshot3)
            {
                heroStats.AddShootingModifierThird(modifier.Code);
                log.Debug($"Добавлен модификатор третьего этапа: {modifier.Code}");
                break; // Добавляем только один модификатор
            }
        }
    }

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

        heroStats.SetStat(statName, value);
        ApplySpecialEffects(statName);        

        if (UIManager.Instance == null)
        {
            log.Error("UIManager не найден!");
            return;
        }

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

        if (characterCode == CharacterStatCode.DropRadius && colliderDropRadius != null)
        {
            colliderDropRadius.radius = heroStats.DropRadius;
            DrawCircle();
            log.Debug($"DropRadius обновлён: {heroStats.DropRadius}");
        }
    }

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

    public void UpgradeUsableDots(DotCode code, float? value, bool typeUpgradeDmgDot)
    {
        bool seacrhDot = true; // Чек нашли ли мы нужный нам дот
        for (int i = 0; i < heroStats.GetUsableDots().Count; i++)
        {
            if (heroStats.GetUsableDots()[i].Code == code)
            {
                if (typeUpgradeDmgDot)
                {
                    if (heroStats.GetUsableDots()[i].DotDur == 0)
                    {
                        heroStats.GetUsableDots()[i].DotDur = ImprovableCharactesDictionary.GetFinalDotDurOfCode(code.ToString());
                        log.Warn("DotDur = 0, code = " + code);
                    }
                    heroStats.GetUsableDots()[i].DotDmg += (int) value;
                    log.Debug("Улучшили dot " + heroStats.GetUsableDots()[i].Code + ", dmg на " + value + ", округлили до " + (int)value + ", теперь он = " + heroStats.GetUsableDots()[i].DotDmg);
                } else
                {
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

        if (seacrhDot)
        {
            log.Debug("Дот не был найден в списке usableDotsArray героя, code = " + code);
            AddDotInUsableDotsArrayOfCode(code);
        }

        UIManager.Instance.printUsableDots(heroStats.GetUsableDots());
    }

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
            log.Debug("IncrementLvl. Увелечение характеристики : "+ characteristic.Key + " на " + characteristic.Value);
            SetStat(characteristic.Key.ToString(), characteristic.Value);
        }
    }

    void RotateTowardsMouse()
    {
        // Получаем позицию курсора в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        // Вычисляем направление от игрока к курсору
        Vector2 direction = mousePosition - transform.position;

        // Вычисляем угол для поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Устанавливаем новый угол поворота
        rb.rotation = angle; 
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
        UIManager.Instance.printActualHP(heroStats.ActualHP);
    }

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