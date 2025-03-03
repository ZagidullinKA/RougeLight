using UnityEngine;
using log4net;
using Mono.Cecil.Cil;
using static UnityEngine.Rendering.DebugUI;


public class Hero : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Hero));

    public Rigidbody2D rb;
    public Shooting shooting;
    public Transform firePoint;
    public GameObject Bullet;
    private Vector2 moveVector;
    private bool isShooting = false;

    //Переменная для godmod
    public bool isGodMode = false;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shooting = GetComponent<Shooting>();
        isEnemy = false; // Герой не является врагом
        InitializeCharacteristicsAndDots();
        base.Awake();
        UIManager.Instance.printActualHP(actualHP);

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = dropRadius;
            collider.isTrigger = true; // Делаем коллайдер триггером
        } else
        {
            log.Error("CircleCollider2D is null");
        }
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

            SetCharacteristic(drop.Code, drop.Update);
            UIManager.Instance.printCharacters(maxHP, dmg, atkSpeed,
            moveSpeed, luck, critChance, evadeChance, armor, debuffResist,
            Vampire, hpFromDropRestore, dropRadius, bulletFlySpeed, bulletTimeAlive);

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

    protected override void Die()
    {
        if (!isGodMode)
        {
            base.Die();
        } else
        {
            log.Debug("Ты бы умер, но ты либо тестер, либо читер");
            actualHP = maxHP;
            UIManager.Instance.printActualHP(actualHP);
        }
    }

}