using log4net;
using UnityEngine;

public class Mobs : Character, IAttacker, IMovable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Mobs));

    protected override void Start()
    {
        base.Start();
        isEnemy = true; // Моб является врагом
        //InitializeCharacteristics();
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

    /*
    // Инициализация характеристик
    private void InitializeCharacteristics()
    {
        // Используем справочник врагов
        var enemyData = EnemyTypes.items.Find(x => x.type == enemyType);
        if (enemyData != null)
        {
            maxHP = enemyData.maxHP;
            dmg = enemyData.dmg;
            atkSpeed = enemyData.atkSpeed;
            moveSpeed = enemyData.moveSpeed;
            luck = enemyData.luck;
            critChance = enemyData.critChance;
            evadeChance = enemyData.evadeChance;
            armor = enemyData.armor;
            debuffResist = enemyData.debuffResist;
            vampire = enemyData.vampire;
            hpFromDropRestore = enemyData.hpFromDropRestore;
            dropRadius = enemyData.dropRadius;
        }
    }
    */
}
