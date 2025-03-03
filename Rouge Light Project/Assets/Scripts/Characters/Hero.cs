using UnityEngine;
using log4net;


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
    CircleCollider2D colliderDropRadius;
    private LineRenderer lineRendererDropRadius;

    //Переменная для godmod
    public bool isGodMode = false;

    protected override void Awake()
    {
        // Заглушка ебаная
        usableDotsArray.Add(new DotEffect("fire1", 1, 1, 1, 1));
        // Конец заглушки ебаной

        log.Debug(usableDotsArray[0].code);
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        shooting = GetComponent<Shooting>();
        isEnemy = false; // Герой не является врагом

        colliderDropRadius = GetComponent<CircleCollider2D>();
        if (colliderDropRadius != null)
        {
            colliderDropRadius.isTrigger = true; // Делаем коллайдер триггером

            //Делаем настройки для визуализации радиуса дропа

            lineRendererDropRadius = gameObject.AddComponent<LineRenderer>();

            // Настройка LineRenderer
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

        InitializeCharacteristics();
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

    void DrawCircle()
    {
        int segments = 50; // Количество сегментов для окружности
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что объект можно подобрать
        if (other.CompareTag("Drop"))
        {
            log.Debug("Предмет подобрали! - " + other.name);
            Drop drop = other.GetComponent<Drop>();
            SetCharacteristic(drop.Code, drop.Update);
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
    private void InitializeCharacteristics()
    {
        // Используем справочник всех характеристик
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            if (item.Upgradable)
            {
                // Получаем улучшаемые характеристики из справочника улучшаемых характеристик
                var upgradableItem = ImprovableCharactesDictionary.GetAllImprovableCharacteristics().Find(x => x.Code == item.Code);
                if (upgradableItem != null)
                {
                    SetCharacteristic(item.Code, upgradableItem.FinalValue);
                }
            }
            else
            {
                // Используем базовые значения из справочника всех характеристик
                SetCharacteristic(item.Code, item.BaseAmount);
            }
        }
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
            case "evadeChace":
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
        }
    }

}