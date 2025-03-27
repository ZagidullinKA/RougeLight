using DG.Tweening;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;
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
    public List<RecievedDotEffect> recievedDotsArray = new List<RecievedDotEffect>();
    protected float lastHandlingAppliedDoTEffectsTime = 0;
    protected float handlingAppliedDoTEffectsPeriod = 1f;

    // Массив для хранения наносимых снарядом ДОТов (Damage Over Time)
    public List<UsableDotEffect> usableDotsArray = new List<UsableDotEffect>();


    // Инициализация
    protected virtual void Awake()
    {
        actualHP = maxHP; // Устанавливаем текущее здоровье на максимальное при старте
    }

    public virtual void SetCharacteristic(string code, float? value)
    {
        log.Error("Метод только для переопределения в дочерних классах и удобства вызова из этого класса. Метод не переопределен");
    }

    public virtual float? GetCharacteristic(string code)
    {
        log.Error("Метод только для переопределения в дочерних классах и удобства вызова из этого класса. Метод не переопределен");
        return null;
    }

    // Реализация IDamageable
    public virtual void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        int damageAfterArmor = damage - armor;
        if (damageAfterArmor < 0) damageAfterArmor = 0;
        actualHP -= damageAfterArmor;
        if (isEnemy)
        {
            if (typeDamage == TypeOfDamage.TYPE_ATTACK)
                UIManager.Instance.printDamage(damageAfterArmor.ToString(), transform.position);
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
        float randomValue = UnityEngine.Random.value; // Генерация случайного числа от 0 до 1
        float evadeChanceMoment = 1 / evadeChance;
        log.Debug(randomValue);
        log.Debug(randomValue < evadeChanceMoment);
        return randomValue > evadeChanceMoment;
    }

    public void TakeDots(List<UsableDotEffect> forcedDotsArray)
    {
        if (forcedDotsArray.Count == 0) { return; }
        log.Debug("HandlingAppliedDoTEffects. Вошли в получение дотов от патрона");

        foreach (var itemForcedDots in forcedDotsArray)
        {
            bool checkAvailability = true;
            foreach (var itemRecievedDot in recievedDotsArray)
            {
                if (itemForcedDots.Code == itemRecievedDot.Code)
                {
                    itemRecievedDot.DotDur = itemRecievedDot.DotDur;
                    itemRecievedDot.DotDmg += 1;
                    itemRecievedDot.Count += 1;
                    itemRecievedDot.Tick = 0;
                    checkAvailability = false;
                    break;
                }
            }
            if (checkAvailability) {
                log.Debug("HandlingAppliedDoTEffects. Добавляем дот itemForcedDots.Code = " + itemForcedDots.Code);
                log.Debug("HandlingAppliedDoTEffects. itemForcedDots.DotDmg = " + itemForcedDots.DotDmg);
                recievedDotsArray.Add(new RecievedDotEffect(itemForcedDots));                   
            }
        }
    }

    public class ItemPrintDot
    {
        public string code;
        public int damage;

        public ItemPrintDot (string code, int damage)
        {
            this.code = code;
            this.damage = damage;
        }
    }

    public void HandlingAppliedDoTEffects()
    {
        if (recievedDotsArray.Count <= 0)
        {
            lastHandlingAppliedDoTEffectsTime = Time.time;
            return;
        }

        if (Time.time - lastHandlingAppliedDoTEffectsTime < handlingAppliedDoTEffectsPeriod)
        {
            return;
        }

        

        log.Debug("HandlingAppliedDoTEffects. Вошли в обработку дотов");
        lastHandlingAppliedDoTEffectsTime = Time.time;
        List<RecievedDotEffect> removeRecievedDotsArray = new List<RecievedDotEffect>();
        List<ItemPrintDot> takeDamageList = new List<ItemPrintDot>();
        foreach (var itemRecievedDot in recievedDotsArray)
        {
            log.Debug("HandlingAppliedDoTEffects. Обрабатываем itemRecievedDot.Code = " + itemRecievedDot.Code);

            int countedDotDmg = 0;
            switch (itemRecievedDot.Type)
            {
                case TypeOfDots.TYPE_PERCENT:
                    countedDotDmg = (int)Math.Ceiling((float)(maxHP / 100 * itemRecievedDot.DotDmg));
                    log.Debug("HandlingAppliedDoTEffects. countedDotDmg = " + countedDotDmg + ", (int)Math.Ceiling((float)(maxHP / 100 * item.DotDmg)) = " + (int)Math.Ceiling((float)(maxHP / 100 * itemRecievedDot.DotDmg)));
                    log.Debug("HandlingAppliedDoTEffects. item.DotDmg = " + itemRecievedDot.DotDmg);
                    break;
                case TypeOfDots.TYPE_FIXED:
                case TypeOfDots.TYPE_BASE_DMG_PERCENT:
                    countedDotDmg = itemRecievedDot.DotDmg;
                    break;
                default:
                    log.Error("HandlingAppliedDoTEffects. Такого типа дота не Существует!");
                    break;
            }

            if (itemRecievedDot.AffectedChar == "actualHP")
            {
                takeDamageList.Add(new ItemPrintDot(itemRecievedDot.Code, countedDotDmg));
                TakeDamage(countedDotDmg, TypeOfDamage.TYPE_DOT);
                if (itemRecievedDot.DotDur <= 1)
                {
                    removeRecievedDotsArray.Add(itemRecievedDot);
                } else
                {
                    itemRecievedDot.DotDur--;
                }

                continue;
            }

            float? affectedCharCurrentvalue = GetCharacteristic(itemRecievedDot.AffectedChar);
            if (itemRecievedDot.Tick < 1)
            {
                log.Debug("HandlingAppliedDoTEffects. item.AffectedChar = " + itemRecievedDot.AffectedChar + ", countedDotDmg = " + -countedDotDmg);
                if (affectedCharCurrentvalue == null)
                {
                    log.Error("У характеристики нет значения item.AffectedChar = " + itemRecievedDot.AffectedChar);
                }
                if (affectedCharCurrentvalue - countedDotDmg < 0)
                {
                    countedDotDmg = (int)affectedCharCurrentvalue;
                }

                SetCharacteristic(itemRecievedDot.AffectedChar, -countedDotDmg);
                itemRecievedDot.AffectedDamage += countedDotDmg;
            }
            itemRecievedDot.Tick++;
            

            
            log.Debug("HandlingAppliedDoTEffects.item.DotDur = " + itemRecievedDot.DotDur);
            if (itemRecievedDot.DotDur <= 1)
            {
                log.Debug("HandlingAppliedDoTEffects. удаляем и возвращаем характеристику");
                SetCharacteristic(itemRecievedDot.AffectedChar, itemRecievedDot.AffectedDamage);
                removeRecievedDotsArray.Add(itemRecievedDot);
            }
            else
            {
                itemRecievedDot.DotDur--;
            }

        }

        if (takeDamageList.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < takeDamageList.Count; i++)
            {
                var item = takeDamageList[i];
                sb.Append(item.code + " : " + item.damage);

                if (i < takeDamageList.Count - 1)
                {
                    sb.Append(" | ");
                }
            }
            UIManager.Instance.printDamage(sb.ToString(), transform.position);

            takeDamageList.Clear();
        }

        recievedDotsArray.RemoveAll(item => removeRecievedDotsArray.Contains(item));
        removeRecievedDotsArray.Clear();   
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