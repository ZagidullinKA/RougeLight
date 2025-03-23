using UnityEngine;
using log4net;
using Mono.Cecil.Cil;
using static UnityEngine.Rendering.DebugUI;
using System.Reflection.Emit;
using static UnityEditor.Progress;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;


public class Hero : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Hero));

    public Rigidbody2D rb;
    public Shooting shooting;
    public Transform firePoint;
    private Vector2 moveVector;
    private bool isShooting = false;

    private CircleCollider2D colliderDropRadius;
    private LineRenderer lineRendererDropRadius;

    private const float UpgradeLvlFactor = 1.5f;

    //Переменная для godmod
    public bool isGodMode = false;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shooting = GetComponent<Shooting>();
        isEnemy = false; // Герой не является врагом
        

        colliderDropRadius = GetComponent<CircleCollider2D>();
        if (colliderDropRadius != null)
        {
            colliderDropRadius.isTrigger = true; // Äåëàåì êîëëàéäåð òðèããåðîì

            //Äåëàåì íàñòðîéêè äëÿ âèçóàëèçàöèè ðàäèóñà äðîïà

            lineRendererDropRadius = gameObject.AddComponent<LineRenderer>();

            // Íàñòðîéêà LineRenderer
            lineRendererDropRadius.startWidth = 0.01f;
            lineRendererDropRadius.endWidth = 0.01f;
            lineRendererDropRadius.useWorldSpace = false;
            lineRendererDropRadius.material = new Material(Shader.Find("Sprites/Default"));
            lineRendererDropRadius.startColor = Color.green;
            lineRendererDropRadius.endColor = Color.green;
        }
        else
        {
            log.Error("CircleCollider2D DropRadius is null");
        }


        InitializeCharacteristicsAndDots();
        base.Awake();
        UIManager.Instance.printActualHP(actualHP);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UIManager.Instance.printActualHP(actualHP);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            isShooting = true;
        }
    }

    void DrawCircle()
    {
        int segments = 50; // Êîëè÷åñòâî ñåãìåíòîâ äëÿ îêðóæíîñòè
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
        Shoot();
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
                case "character":
                    SetCharacteristic(drop.ItemCode, drop.Update);
                    break;
                case "dot":
                    UpgradeUsableDots(drop.ItemCode, drop.Update, (bool) drop.IsDmgUpIfDot);
                    break;
                case "money":
                    Observer.increaseMoney(drop.Update);
                    break;
                case "heal":
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



    // Реализация IAttacker
    public void Shoot()
    {
        if (isShooting)
        {
            shooting.Shot(dmg, critChance, atkSpeed, bulletFlySpeed, bulletTimeAlive, usableDotsArray);
            isShooting = false;
        }
    }

    // Реализация IMovable
    public void Move()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");

        rb.MovePosition(rb.position + moveVector * moveSpeed * Time.deltaTime);
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
                var upgradableItem = ImprovableCharactesDictionary.GetAllImprovableCharacteristicsAndDots().Find(x => x.Code == item.Code);
                if (upgradableItem is null)
                {
                    log.Error("upgradableItem is null - item.code = " + item.Code);
                    continue;
                }
                SetCharacteristic(item.Code, upgradableItem.FinalValue);
            }
            else
            {
                // Используем базовые значения из справочника всех характеристик
                SetCharacteristic(item.Code, item.BaseAmount);
            }
        }

        foreach (var item in ImprovableCharactesDictionary.GetAllImprovableDots())
        {
            ValidationValue.ValidateIntNotNull(
                        (item.FinalDotDmg, "upgradableItem.FinalDotDmg"),
                        (item.FinalDotDur, "upgradableItem.FinalDotDur"),
                        (item.DmgUpgradeAmount, "upgradableItem.DmgUpgradeAmount"),
                        (item.DurationUpgradeAmount, "upgradableItem.DurationUpgradeAmount")
                    );

            usableDotsArray.Add(new DotEffect(item.Code, (int)item.FinalDotDmg, (int)item.FinalDotDur,
                (int)item.DmgUpgradeAmount, (int)item.DurationUpgradeAmount));
        }
        UIManager.Instance.printDots(usableDotsArray);
    }

    // Установка значения характеристики
    public void SetCharacteristic(string code, float? value)
    {
        ValidationValue.ValidateFloatNotNull(
            (value, code)
            );

        log.Debug("SetCharacteristic :" + code + " - " + value);
        switch (code)
        {
            case "maxHP":
                maxHP += (int)value;
                break;
            case "dmg":
                dmg += (int)value;
                break;
            case "atkSpeed":
                atkSpeed += (float) value;
                break;
            case "moveSpeed":
                moveSpeed += (int)value;
                break;
            case "luck":
                luck += (int)value;
                break;
            case "critChance":
                critChance += (int)value;
                break;
            case "evadeChance":
                evadeChance += (int)value;
                break;
            case "armor":
                armor += (int)value;
                break;
            case "debuffResist":
                debuffResist += (int)value;
                break;
            case "Vampire":
                vampire += (int)value;
                break;
            case "hpFromDropRestore":
                hpFromDropRestore += (int)value;
                break;
            case "dropRadius":
                dropRadius += (int)value;
                colliderDropRadius.radius = dropRadius;
                DrawCircle();
                break;
            case "bulletFlySpeed":
                bulletFlySpeed += (int)value;
                break;
            case "bulletTimeAlive":
                bulletTimeAlive += (int)value;
                break;
            default:
                log.Warn($"Неизвестная характеристика: {code}");
                break;
        }

        if (UIManager.Instance == null)
        {
            log.Error("UIManager не найден!");
            return;
        }

        UIManager.Instance.printCharacters(maxHP, dmg, atkSpeed,
            moveSpeed, luck, critChance, evadeChance, armor, debuffResist,
            Vampire, hpFromDropRestore, dropRadius, bulletFlySpeed, bulletTimeAlive);
    }

    public void AddDotInUsableDotsArrayOfCode(string code)
    {
        var item = ImprovableCharactesDictionary.GetImprovableCharacteristicOrDot(code);

        if (item == null)
        {
            log.Error("AddDotInUsableDotsArrayOfCode. дот который пытаемся добавить не найден в справочнике, code = " + code);
            return;
        }

        usableDotsArray.Add(new DotEffect(item.Code, (int)item.FinalDotDmg, (int)item.FinalDotDur,
                (int)item.DmgUpgradeAmount, (int)item.DurationUpgradeAmount));
    }

    public void UpgradeUsableDots(string code, float? value, bool typeUpgradeDmgDot)
    {
        bool seacrhDot = true; // Чек нашли ли мы нужный нам дот
        for (int i = 0; i < usableDotsArray.Count; i++)
        {
            if (usableDotsArray[i].code == code)
            {
                if (typeUpgradeDmgDot)
                {
                    if (usableDotsArray[i].DotDur == 0)
                    {
                        usableDotsArray[i].DotDur = ImprovableCharactesDictionary.GetFinalDotDurOfCode(code);
                        log.Warn("DotDur = 0, code = " + code);
                    }
                    usableDotsArray[i].DotDmg += (int) value;
                    log.Debug("Улучшили dot " + usableDotsArray[i].code + ", dmg на " + value + ", округлили до " + (int)value + ", теперь он = " + usableDotsArray[i].DotDmg);
                } else
                {
                    if (usableDotsArray[i].DotDmg == 0)
                    {
                        usableDotsArray[i].DotDmg = ImprovableCharactesDictionary.GetFinalDotDmgOfCode(code);
                        log.Warn("DotDmg = 0, code = " + code);
                    }
                    usableDotsArray[i].DotDur += (int)value;
                    log.Debug("Улучшили dot " + usableDotsArray[i].code + ", dur на " + value + ", округлили до " + (int)value + ", теперь он = " + usableDotsArray[i].DotDur);
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

        UIManager.Instance.printDots(usableDotsArray);
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
        var upgrades = new Dictionary<string, float>
        {
            ["dmg"] = CalculateUpgrade(lvl),
            ["atkSpeed"] = CalculateUpgrade(lvl),
            ["maxHP"] = CalculateUpgrade(lvl)
        };

        // 3. Применяем характеристики в цикле
        foreach (var characteristic in upgrades)
        {
            log.Debug("IncrementLvl. Увелечение характеристики : "+ characteristic.Key + " на " + characteristic.Value);
            SetCharacteristic(characteristic.Key, characteristic.Value);
        }
    }

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

    public override void Heal(int amount)
    {
        base.Heal(amount);
        UIManager.Instance.printActualHP(actualHP);
    }

    protected override void Die()
    {

        if (!isGodMode)
        {
            if (isCanDie)
            {
                isCanDie = false;
                base.Die();
            }
        }
            else
        {
            log.Debug("Ты бы умер, но ты либо тестер, либо читер");
            actualHP = maxHP;
            UIManager.Instance.printActualHP(actualHP);
        }
    }

}