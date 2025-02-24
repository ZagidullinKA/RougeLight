using log4net;
using Mono.Cecil.Cil;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Mobs : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Mobs));

    private Rigidbody2D rb;
    private DropManager dropManagerScript;

    private Transform player; // Ссылка на игрока
    public float rotationSpeed = 5f; // Скорость поворота

    protected override void Start()
    {
        InitializeCharacteristics("UnitTest");
        base.Start();
        isEnemy = true; // Моб является врагом

        dropManagerScript = GetComponent<DropManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null)
        {
            RotateTowardsPlayer();
        }
    }

    // Реализация IAttacker
    public void Shoot()
    {
        log.Debug("Моб стреляет с пониженной точностью!");
    }

    // Реализация IMovable
    public void Move()
    {
        log.Debug("Моб движется со скоростью " + moveSpeed);
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    // Инициализация характеристик
    private void InitializeCharacteristics(string enemyType)
    {
        // Используем справочник врагов
        var enemyData = EnemyTypesDictionary.GetCharacteristic(enemyType);
        if (enemyData != null)
        {
            maxHP = enemyData.MaxHP;
            dmg = enemyData.Dmg;
            atkSpeed = enemyData.AtkSpeed;
            moveSpeed = enemyData.MoveSpeed;
            critChance = enemyData.CritChance;
            evadeChance = enemyData.EvadeChance;
            armor = enemyData.Armor;
            debuffResist = enemyData.DebuffResist;
            vampire = enemyData.Vampire;
            bulletFlySpeed = enemyData.BulletflySpeed;
            bulletTimeAlive = enemyData.BulletTimeAlive;

            log.Debug("Характеристики моба - " + enemyData.MaxHP );
        }
    }

    protected override void Die()
    {
        dropManagerScript.DropLoss();
        base.Die();
    }
}
