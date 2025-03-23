using DG.Tweening;
using log4net;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class Character : MonoBehaviour, IDamageable, IHealable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Character));
    protected bool isCanDie = true;           // Можети ли умереть 

    // Основные характеристики
    protected int maxHP = 0;
    protected int dmg = 0;
    protected float? atkSpeed = 0;
    protected int moveSpeed = 0;
    protected int luck = 0;
    protected int critChance = 0;
    protected int evadeChance = 0;
    protected int armor = 0;
    protected int debuffResist = 0;
    protected int vampire = 0;
    protected int hpFromDropRestore = 0;
    protected int dropRadius = 0;
    protected int bulletFlySpeed = 0;
    protected int bulletTimeAlive = 0;

    // Свойства с get для доступа к переменным
    public int MaxHP => maxHP;
    public int Dmg => dmg;
    public float? AtkSpeed => atkSpeed;
    public int MoveSpeed => moveSpeed;
    public int Luck => luck;
    public int CritChance => critChance;
    public int EvadeChance => evadeChance;
    public int Armor => armor;
    public int DebuffResist => debuffResist;
    public int Vampire => vampire;
    public int HpFromDropRestore => hpFromDropRestore;
    public int DropRadius => dropRadius;
    public int BulletFlySpeed => bulletFlySpeed;
    public int BulletTimeAlive => bulletTimeAlive;

    // Текущее здоровье
    public int actualHP = 0;

    // Флаг, определяющий, является ли объект врагом
    public bool isEnemy = false;

    // Массив для хранения полученных ДОТов (Damage Over Time)
    public List<DotEffect> recievedDots = new List<DotEffect>();

    // Массив для хранения наносимых снарядом ДОТов (Damage Over Time)
    public List<DotEffect> usableDotsArray = new List<DotEffect>();

    public GameObject damageTextPrefab;

    // Инициализация
    protected virtual void Awake()
    {
        damageTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DamageTextPrefab.prefab");

        if (damageTextPrefab == null)
        {
            log.Error("Awake. Префаб DamageTextPrefab не найден по указанному пути.");
        }


        actualHP = maxHP; // Устанавливаем текущее здоровье на максимальное при старте
    }

    // Реализация IDamageable
    public virtual void TakeDamage(int damage)
    {
        int damageAfterArmor = damage - armor;
        if (damageAfterArmor < 0) damageAfterArmor = 0;
        actualHP -= damageAfterArmor;
        if (isEnemy)
        {
            log.Debug("Вошли в условие отображения урона!");
            // Создаем текст с уроном

            if (damageTextPrefab == null)
            {
                log.Error("TakeDamage. Префаб DamageTextPrefab не найден по указанному пути.");
            }

            

            GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            // Устанавливаем текст
            TextMeshPro textComponent = damageText.GetComponent<TextMeshPro>();
            if (textComponent == null)
            {
                log.Error("TakeDamage. Префаб textComponent не найден.");
            }

            textComponent.text = damageAfterArmor.ToString();
            textComponent.color = Color.red;
            textComponent.sortingOrder = 100;

            // Плавно поднимаем текст вверх
            damageText.transform.DOMoveY(transform.position.y + 2f, 2f)
                .SetEase(Ease.OutQuad); // Плавное ускорение и замедление

            // Плавно изменяем прозрачность текста
            textComponent.DOFade(0f, 2f)
                .OnComplete(() => Destroy(damageText));
        }

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
}