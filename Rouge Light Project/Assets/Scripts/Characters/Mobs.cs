using log4net;
using System;
using UnityEngine;

public class Mobs : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Mobs));
    private int idMob;

    public int IdMob
    {
        get => idMob;
        set { idMob = value; }
    }

    private int deathPrice;
    public int DeathPrice
    {
        get => deathPrice;
        set { deathPrice = value; }
    }

    private Rigidbody2D rb;
    private DropManager dropManagerScript;

    private Transform player; // Ссылка на игрока
    public float rotationSpeed = 5f; // Скорость поворота

    private Shooting shooting;

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

        base.HandlingAppliedDoTEffects();
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
            bulletFlySpeed = (int) Math.Round(enemyData.BulletFlySpeed * mobsMultipier);
            bulletTimeAlive = (int)Math.Round(enemyData.BulletTimeAlive * mobsMultipier);
            deathPrice = enemyData.DeathPrice;

            log.Debug("InitializeCharacteristics. Характеристики моба - " + enemyData.DeathPrice);
        }
    }

    public override void SetCharacteristic(string code, float? value)
    {
        ValidationValue.ValidateFloatNotNull(
            (value, code)
            );

        log.Debug("SetCharacteristic :" + code + " - " + value);
        switch (code)
        {
            case "maxHP":
                maxHP += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + maxHP);
                break;
            case "dmg":
                dmg += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + dmg);
                break;
            case "atkSpeed":
                atkSpeed += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + atkSpeed);
                break;
            case "moveSpeed":
                moveSpeed += (int)value;
                EnemyTestAI mobsMoveScript = gameObject.GetComponent<EnemyTestAI>();
                mobsMoveScript.moveSpeed = moveSpeed;
                log.Debug("SetCharacteristic теперь " + code + " = " + moveSpeed);
                break;
            case "critChance":
                critChance += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + critChance);
                break;
            case "evadeChance":
                evadeChance += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + evadeChance);
                break;
            case "armor":
                armor += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + armor);
                break;
            case "debuffResist":
                debuffResist += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + debuffResist);
                break;
            case "vampire":
                vampire += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + vampire);
                break;
            case "bulletFlySpeed":
                bulletFlySpeed += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + bulletFlySpeed);
                break;
            case "bulletTimeAlive":
                bulletTimeAlive += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + bulletTimeAlive);
                break;
            case "deathPrice":
                deathPrice += (int)value;
                log.Debug("SetCharacteristic теперь " + code + " = " + deathPrice);
                break;
        }
    }

    public override float? GetCharacteristic(string code)
    {

        log.Debug("GetCharacteristic :" + code);
        switch (code)
        {
            case "maxHP":
                return maxHP;
            case "dmg":
                return dmg;
            case "atkSpeed":
                return atkSpeed;
            case "moveSpeed":
                return moveSpeed;
            case "critChance":
                return critChance;
            case "evadeChance":
                return evadeChance;
            case "armor":
                return armor;
            case "debuffResist":
                return debuffResist;
            case "Vampire":
                return vampire;
            case "bulletFlySpeed":
                return bulletFlySpeed;
            case "bulletTimeAlive":
                return bulletTimeAlive;
            case "deathPrice":
                return deathPrice;
            default:
                log.Warn($"Неизвестная характеристика: {code}");
                return null;
        }
    }

    public override void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        log.Debug("TakeDamage. Die. idMob = " + idMob);
        base.TakeDamage(damage, typeDamage);
    }

    protected override void Die()
    {
        Destroy(GetComponent<BoxCollider2D>());
        if (isCanDie)
        {
            isCanDie = false;
            log.Debug("Die. idMob = " + idMob);
            dropManagerScript.DropLoss();
            Observer.IncrementCountKill(DeathPrice);
            base.Die();
        }
        
    }
}
