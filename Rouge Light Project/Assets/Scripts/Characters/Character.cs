using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Базовый абстрактный класс для всех персонажей, реализующий интерфейсы получения урона и лечения
public abstract class Character : MonoBehaviour, IDamageable, IHealable
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Character));
    // Добавляем переменную хранящую все характеристики
    [SerializeField] public BaseStats stats;

    // Время последней обработки периодических эффектов (Damage over Time)
    protected float lastHandlingAppliedDoTEffectsTime = 0;
    // Период между обработками эффектов (по умолчанию 1 секунда)
    protected float handlingAppliedDoTEffectsPeriod = 1f;

    // Инициализация
    protected virtual void Awake()
    {
        if (stats != null)
        {
            stats.InitializeFromDictionary(new DictionaryCharacters());
        }

        stats.ActualHP = stats.MaxHP; // Устанавливаем текущее здоровье на максимальное при старте
    }

    // Возвращает значение числовой характеристики через рефлексию
    public virtual float? GetStat(string statName)
    {
        if (stats == null)
        {
            log.Debug("Stats не назначены!"); // Логируем отсутствие stats
            return null;
        }

        return stats.GetStat(statName);
    }

    public virtual void SetStat(string code, float? value)
    {
        if (stats == null)
        {
            log.Debug("Stats не назначены!");
            return;
        }

        //Проверям есть ли такой code в переменных statCode
        if (Enum.TryParse<CharacterStatCode>(code, ignoreCase: true, out CharacterStatCode statCode))
        {
            stats.SetStat(code, value); // Присваиваем новое значение переменной
            ApplySpecialEffects(code);  // На случай необходимости дополнительных изменений, кроме самой переменной
        }
    }

    // Метод для применения дополнительных эффектов при изменении характеристики
    protected virtual void ApplySpecialEffects(string statName)
    {
        log.Debug("Пока только переопределение");
    }

    // Основной метод получения урона
    protected virtual void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        // Уменьшаем текущее здоровье
        stats.ActualHP -= damage;

        // Для врагов отображаем урон в UI
        if (stats.IsEnemy)
        {
            if (typeDamage == TypeOfDamage.TYPE_ATTACK)
                UIManager.Instance.printDamage(damage.ToString(), transform.position);
        }

        log.Debug($"Противник получил урон: {damage}. Осталось здоровья: {stats.ActualHP}");
        if (stats.ActualHP <= 0)
        {
            Die();
        }
    }

    // Расчет конечного урона с учетом брони
    public virtual void CalculateDamageAfterArmor(int damage, TypeOfDamage typeDamage)
    {
        int damageAfterArmor = damage - stats.Armor;
        if (damageAfterArmor < 0) damageAfterArmor = 0;
        TakeDamage(damageAfterArmor, typeDamage);
    }

    // Метод проверки вероятности уклонения
    public bool TryDodge()
    {
        float randomValue = UnityEngine.Random.value; // Генерация случайного числа от 0 до 1
        float evadeChanceMoment = 1 / stats.EvadeChance;
        log.Debug(randomValue);
        log.Debug(randomValue < evadeChanceMoment);
        return randomValue > evadeChanceMoment;
    }

    // Применение эффектов Damage over Time к персонажу
    public void TakeDots(List<UsableDotEffect> forcedDotsArray)
    {
        if (forcedDotsArray.Count == 0) { return; }
        log.Debug("HandlingAppliedDoTEffects. Вошли в получение дотов от патрона");

        // Обработка каждого полученного эффекта
        foreach (var itemForcedDots in forcedDotsArray)
        {
            bool checkAvailability = true;
            // Проверяем, есть ли уже такой эффект
            foreach (var itemRecievedDot in stats.GetRecievedDots())
            {
                if (itemForcedDots.Code == itemRecievedDot.Code)
                {
                    // Обновляем параметры существующего эффекта
                    itemRecievedDot.DotDur = itemForcedDots.DotDur;
                    itemRecievedDot.DotDmg += 1;
                    itemRecievedDot.Count += 1;
                    itemRecievedDot.Tick = 0;
                    checkAvailability = false;
                    break;
                }
            }
            if (checkAvailability)
            {
                log.Debug("HandlingAppliedDoTEffects. Добавляем дот itemForcedDots.Code = " + itemForcedDots.Code);
                log.Debug("HandlingAppliedDoTEffects. itemForcedDots.DotDmg = " + itemForcedDots.DotDmg);
                stats.SetRecievedDots(new RecievedDotEffect(itemForcedDots));
            }
        }
    }

    // Вспомогательный класс для форматированного вывода информации о DoT-эффектах
    public class ItemPrintDot
    {
        public string code;
        public int damage;

        public ItemPrintDot(string code, int damage)
        {
            this.code = code;
            this.damage = damage;
        }
    }

    // Обработка активных DoT-эффектов
    public void HandlingAppliedDoTEffects()
    {
        if (stats.GetRecievedDots().Count <= 0)
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

        // Обработка каждого активного эффекта
        foreach (var itemRecievedDot in stats.GetRecievedDots())
        {
            log.Debug("HandlingAppliedDoTEffects. Обрабатываем itemRecievedDot.Code = " + itemRecievedDot.Code);

            int countedDotDmg = 0;
            // Расчет урона в зависимости от типа эффекта
            switch (itemRecievedDot.Type)
            {
                case TypeOfDots.TYPE_PERCENT:
                    countedDotDmg = (int)Math.Ceiling((float)(stats.MaxHP / 100 * itemRecievedDot.DotDmg));
                    break;
                case TypeOfDots.TYPE_FIXED:
                case TypeOfDots.TYPE_BASE_DMG_PERCENT:
                    countedDotDmg = itemRecievedDot.DotDmg;
                    break;
                default:
                    log.Error("HandlingAppliedDoTEffects. Такого типа дота не Существует!");
                    break;
            }

            // Учет сопротивления к дебаффам
            countedDotDmg -= stats.DebuffResist;

            if (countedDotDmg < 0)
            {
                countedDotDmg = 0;
            }

            // Обработка эффектов, влияющих на здоровье
            if (itemRecievedDot.AffectedChar == CharacterStatCode.ActualHP)
            {
                takeDamageList.Add(new ItemPrintDot(itemRecievedDot.Code.ToString(), countedDotDmg));
                if (countedDotDmg != 0)
                    TakeDamage(countedDotDmg, TypeOfDamage.TYPE_DOT);

                if (itemRecievedDot.DotDur <= 1)
                {
                    removeRecievedDotsArray.Add(itemRecievedDot);
                }
                else
                {
                    itemRecievedDot.DotDur--;
                }

                continue;
            }

            // Обработка эффектов, влияющих на другие характеристики
            float? affectedCharCurrentvalue = GetStat(itemRecievedDot.AffectedChar.ToString());
            if (itemRecievedDot.Tick < 1)
            {
                log.Debug("HandlingAppliedDoTEffects. item.AffectedChar = " + itemRecievedDot.AffectedChar + ", countedDotDmg = " + -countedDotDmg);
                if (affectedCharCurrentvalue == null)
                {
                    log.Error("У характеристики нет значения item.AffectedChar = " + itemRecievedDot.AffectedChar);
                }
                if (affectedCharCurrentvalue - countedDotDmg < 0
                    || (itemRecievedDot.AffectedChar != CharacterStatCode.DebuffResist
                    && itemRecievedDot.AffectedChar != CharacterStatCode.Armor))
                {
                    countedDotDmg = (int)affectedCharCurrentvalue;
                }

                SetStat(itemRecievedDot.AffectedChar.ToString(), -countedDotDmg);
                itemRecievedDot.AffectedDamage += countedDotDmg;
            }
            itemRecievedDot.Tick++;

            // Проверка на завершение действия эффекта
            log.Debug("HandlingAppliedDoTEffects.item.DotDur = " + itemRecievedDot.DotDur);
            if (itemRecievedDot.DotDur <= 1)
            {
                log.Debug("HandlingAppliedDoTEffects. удаляем и возвращаем характеристику");
                SetStat(itemRecievedDot.AffectedChar.ToString(), itemRecievedDot.AffectedDamage);
                removeRecievedDotsArray.Add(itemRecievedDot);
            }
            else
            {
                itemRecievedDot.DotDur--;
            }
        }

        // Отображение информации о полученном уроне от эффектов
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
            if (stats.IsEnemy)
            {
                UIManager.Instance.printDamage(sb.ToString(), transform.position);
            }

            takeDamageList.Clear();
        }

        // Удаление завершившихся эффектов
        stats.RemoveRecievedDots(removeRecievedDotsArray);
        removeRecievedDotsArray.Clear();
    }

    // Реализация IHealable
    public virtual void Heal(int amount)
    {
        stats.ActualHP += amount;
        if (stats.ActualHP > stats.MaxHP)
        {
            stats.ActualHP = stats.MaxHP;
        }
    }

    // Метод для обработки смерти
    protected virtual void Die()
    {
        log.Debug(gameObject.name + " умер.");
        Destroy(gameObject);
    }
}