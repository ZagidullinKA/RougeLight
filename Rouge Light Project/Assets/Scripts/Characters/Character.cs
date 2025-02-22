using log4net;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour, IDamageable, IHealable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Character));


    // Основные характеристики
    protected int maxHP { get; set; }
    protected int dmg { get; set; }
    protected int atkSpeed { get; set; }
    protected int moveSpeed { get; set; }
    protected int luck { get; set; }
    protected int critChance { get; set; }
    protected int evadeChance { get; set; }
    protected int armor { get; set; }
    protected int debuffResist { get; set; }
    protected int vampire { get; set; }
    protected int hpFromDropRestore { get; set; }
    protected int dropRadius { get; set; }
    protected int bulletFlySpeed { get; set; }
    protected int bulletTimeAlive { get; set; }

    // Текущее здоровье
    public int actualHP { get; set; } 

    // Флаг, определяющий, является ли объект врагом
    public bool isEnemy = false;

    // Массив для хранения полученных ДОТов (Damage Over Time)
    public List<DotEffect> recievedDots = new List<DotEffect>();

    // Массив для хранения наносимых снарядом ДОТов (Damage Over Time)
    public List<DotEffect> usableDotsArray = new List<DotEffect>();

    // Инициализация
    protected virtual void Start()
    {
        actualHP = maxHP; // Устанавливаем текущее здоровье на максимальное при старте
    }

    // Реализация IDamageable
    public virtual void TakeDamage(int damage)
    {
        int damageAfterArmor = damage - armor;
        if (damageAfterArmor < 0) damageAfterArmor = 0;
        actualHP -= damageAfterArmor;
        log.Debug("Противник получил урон: " + damageAfterArmor + ". Осталось здоровья: " + actualHP);
        if (actualHP <= 0)
        {
            Die();
        }
    }

    // Метод проверки вероятности уклонения
    public bool TryDodge()
    {
        float randomValue = Random.value; // Генерация случайного числа от 0 до 1
        float evadeChanceMoment = 1 / evadeChance;
        log.Debug(randomValue);
        log.Debug(randomValue < evadeChanceMoment);
        return randomValue > evadeChanceMoment;
    }

    public void TakeDots(List<DotEffect> usableDotsArray)
    {
        foreach (var item in usableDotsArray)
        {
            log.Debug(item.code);
        }
    }

    // Реализация IHealable
    public virtual void Heal(int amount)
    {
        actualHP += amount;
        if (actualHP > maxHP)
        {
            actualHP = maxHP;
        }
    }

    // Метод для обработки смерти
    protected virtual void Die()
    {
        log.Debug(gameObject.name + " умер.");
        Destroy(gameObject);
    }

    // Метод для подбора дропа
    public virtual void PickupDrop()
    {
        Heal(hpFromDropRestore);
        // Здесь можно добавить логику для подбора других предметов
    }
}