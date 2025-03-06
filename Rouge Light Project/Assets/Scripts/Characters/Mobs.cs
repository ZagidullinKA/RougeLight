using log4net;
using System;
using UnityEngine;

public class Mobs : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Mobs));

    private Rigidbody2D rb;
    private DropManager dropManagerScript;

    private Transform player; // Ссылка на игрока
    public float rotationSpeed = 5f; // Скорость поворота

    public Shooting shooting;

    protected override void Awake()
    {
        InitializeCharacteristics("UnitTest");
        base.Awake();
        isEnemy = true; // Моб является врагом

        dropManagerScript = GetComponent<DropManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();

        shooting = gameObject.GetComponent<Shooting>();
    }

    void Update()
    {
        if (player != null)
        {
            RotateTowardsPlayer();
            shooting.Shot(dmg, critChance, atkSpeed, bulletFlySpeed, bulletTimeAlive, usableDotsArray);
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

        ValidationValue.ValidateFloatNotNull(
            (atkSpeed, nameof(atkSpeed))
            );

        ValidationValue.ValidateIntNotNull(
            (maxHP, nameof(maxHP)),
            (dmg, nameof(dmg)),
            (moveSpeed, nameof(moveSpeed)),
            (critChance, nameof(critChance)),
            (evadeChance, nameof(evadeChance)),
            (armor, nameof(armor)),
            (debuffResist, nameof(debuffResist)),
            (vampire, nameof(vampire)),
            (bulletFlySpeed, nameof(bulletFlySpeed)),
            (bulletTimeAlive, nameof(bulletTimeAlive))
            );

        float mobsMultipier = GameGeneration.MobsMultipier;

        if (enemyData != null)
        {
            maxHP = (int) Math.Round(enemyData.MaxHP * mobsMultipier);
            dmg = (int)Math.Round(enemyData.Dmg * mobsMultipier);
            atkSpeed = (float)Math.Round(enemyData.AtkSpeed * mobsMultipier);
            moveSpeed = (int)Math.Round(enemyData.MoveSpeed * mobsMultipier);
            critChance = (int)Math.Round(enemyData.CritChance * mobsMultipier);
            evadeChance = (int)Math.Round(enemyData.EvadeChance * mobsMultipier);
            armor = (int)Math.Round(enemyData.Armor * mobsMultipier);
            debuffResist = (int)Math.Round(enemyData.DebuffResist * mobsMultipier);
            vampire = (int)Math.Round(enemyData.Vampire * mobsMultipier);
            bulletFlySpeed = (int) Math.Round(enemyData.BulletflySpeed * mobsMultipier);
            bulletTimeAlive = (int)Math.Round(enemyData.BulletTimeAlive * mobsMultipier);

            log.Debug("Характеристики моба - " + enemyData.MoveSpeed);
        }
    }

    protected override void Die()
    {
        dropManagerScript.DropLoss();
        base.Die();
    }
}
