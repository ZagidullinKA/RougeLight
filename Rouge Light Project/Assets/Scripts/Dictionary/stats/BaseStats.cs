using UnityEngine;
using System.Collections.Generic;
using System;
using log4net;
using Mono.Cecil.Cil;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Character/BaseStats")]
public class BaseStats : ScriptableObject
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Character));

    [SerializeField] protected int maxHP = 0;                    // Максимальное здоровье персонажа
    [SerializeField] protected int dmg = 0;                      // Урон персонажа
    [SerializeField] protected int meleeDmg = 0;                 // Урон персонажа
    [SerializeField] protected float atkSpeed = 0f;              // Скорость атаки
    [SerializeField] protected int moveSpeed = 0;                // Скорость передвижения
    [SerializeField] protected int critChance = 0;               // Шанс критического удара
    [SerializeField] protected int evadeChance = 0;              // Шанс уворота
    [SerializeField] protected int armor = 0;                    // Броня
    [SerializeField] protected int debuffResist = 0;             // Сопротивление дебаффам
    [SerializeField] protected int vampire = 0;                  // Вампиризм (лечение от урона)
    [SerializeField] protected int hpFromDropRestore = 0;        // Восстановление HP от подбираемых предметов
    [SerializeField] protected int bulletFlySpeed = 0;           // Скорость полёта пули
    [SerializeField] protected int bulletTimeAlive = 0;          // Время жизни пули
    [SerializeField] protected int actualHP = 0;                 // Текущее здоровье персонажа (доступ напрямую)
    [SerializeField] protected bool isCanDie = true;             // Может ли персонаж умереть (доступ напрямую)
    [SerializeField] protected bool isEnemy = false;             // Является ли персонаж врагом (доступ напрямую)
    [SerializeField] protected float rotateSpeed = 0;             // Является ли персонаж врагом (доступ напрямую)
    [SerializeField] protected List<RecievedDotEffect> recievedDotsArray = new List<RecievedDotEffect>(); // Список полученных DoT-эффектов
    [SerializeField] protected List<UsableDotEffect> usableDotsArray = new List<UsableDotEffect>();       // Список используемых DoT-эффектов
    [SerializeField] protected List<TypeOfShootingModifier> shootingModifierFirstArray = new List<TypeOfShootingModifier>();              // Список модификаторов стрельбы 1 этап
    [SerializeField] protected List<TypeOfShootingModifier> shootingModifierSecondArray = new List<TypeOfShootingModifier>();             // Список модификаторов стрельбы 2 этап
    [SerializeField] protected List<TypeOfShootingModifier> shootingModifierThirdArray = new List<TypeOfShootingModifier>();              // Список модификаторов стрельбы 3 этап

    // Свойства с прямым get и set для числовых характеристик
    public int MaxHP { get => maxHP; set => maxHP = value; }
    public int Dmg { get => dmg; set => dmg = value; }
    public int MeleeDmg { get => meleeDmg; set => meleeDmg = value; }
    public float AtkSpeed { get => atkSpeed; set => atkSpeed = value; }
    public int MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public int CritChance { get => critChance; set => critChance = value; }
    public int EvadeChance { get => evadeChance; set => evadeChance = value; }
    public int Armor { get => armor; set => armor = value; }
    public int DebuffResist { get => debuffResist; set => debuffResist = value; }
    public int Vampire { get => vampire; set => vampire = value; }
    public int HpFromDropRestore { get => hpFromDropRestore; set => hpFromDropRestore = value; }
    public int BulletFlySpeed { get => bulletFlySpeed; set => bulletFlySpeed = value; }
    public int BulletTimeAlive { get => bulletTimeAlive; set => bulletTimeAlive = value; }
    public int ActualHP { get => actualHP; set => actualHP = value; }
    public bool IsCanDie { get => isCanDie; set => isCanDie = value; }
    public bool IsEnemy { get => isEnemy; set => isEnemy = value; }
    public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }

    // Устанавливает значение числовой характеристики по её имени через рефлексию
    public virtual void SetStat(string statName, float? value)
    {
        if (value == null)
        {
            log.Error($"Значение для {statName} не указано!");
            return;
        }

        // Проверка существования statName в enum CharacterStatCode
        if (Enum.TryParse<CharacterStatCode>(statName, ignoreCase: true, out CharacterStatCode statCode))
        {
            // Преобразуем CharacterStatCode в имя поля (camelCase)
            string fieldName = char.ToLower(statCode.ToString()[0]) + statCode.ToString().Substring(1);
            // Получаем переменную, соответствующую fieldName
            var field = GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                var currentValue = Convert.ToSingle(field.GetValue(this)); // Преобразуем field в тип данных изменяемой переменной
                var newValue = currentValue + value.Value;
                field.SetValue(this, Convert.ChangeType(newValue, field.FieldType)); // Присваевываем значение новой переменной
                log.Debug($"SetStat: {statName} изменено на {newValue}");
            }
            else
            {
                log.Warn($"Неизвестная характеристика: {statName}");
            }
        }
        else
        {
            log.Warn($"Не удалось преобразовать '{statName}' в CharacterStatCode.");
        }
    }

    // Возвращает значение числовой характеристики по её имени через рефлексию
    public virtual float? GetStat(string statName)
    {

        // Получаем переменную, соответствующую fieldName
        var field = GetType().GetField(statName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field == null)
        {
            log.Debug($"Неизвестная характеристика: {statName}"); // Логируем неизвестную характеристику
            return null;
        }

        return (float?) field.GetValue(this);
    }

    // Добавляет полученный DoT-эффект в список
    public virtual void SetRecievedDots(RecievedDotEffect dotEffect)
    {
        recievedDotsArray.Add(dotEffect);
        log.Debug($"Добавлен RecievedDotEffect: {dotEffect.Code}"); // Логируем добавление
    }

    // Удаляет из списка recievedDotsArray все эффекты, содержащиеся в переданном списке
    public virtual void RemoveRecievedDots(List<RecievedDotEffect> removeRecievedDotsArray)
    {
        int removedCount = recievedDotsArray.RemoveAll(item => removeRecievedDotsArray.Contains(item));
        if (removedCount > 0)
        {
            log.Debug($"Удалено {removedCount} RecievedDotEffect из списка."); // Логируем количество удалённых эффектов
        }
        else
        {
            log.Debug("Не найдено RecievedDotEffect для удаления."); // Логируем, если ничего не удалено
        }
    }

    // Возвращает список полученных DoT-эффектов
    public virtual List<RecievedDotEffect> GetRecievedDots()
    {
        return recievedDotsArray;
    }

    // Добавляет используемый DoT-эффект в список
    public virtual void SetUsableDots(UsableDotEffect usableEffect)
    {
        usableDotsArray.Add(usableEffect);
        log.Debug($"Добавлен UsableDotEffect: {usableEffect.Code}"); // Логируем добавление
    }

    // Возвращает список используемых DoT-эффектов
    public virtual List<UsableDotEffect> GetUsableDots()
    {
        return usableDotsArray;
    }

    // Инициализирует числовые характеристики из справочника
    public virtual void InitializeFromDictionary(DictionaryCharacters dictionary)
    {
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            string fieldName = item.Code.ToString(); // Преобразуем CharacterStatCode в строку
            if (GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic) != null)
            {
                SetStat(item.Code.ToString(), item.BaseAmount - Convert.ToSingle(GetStat(fieldName)));
            }
        }
    }

    // Возвращает список модификаторов стрельбы для первого этапа
    public virtual List<TypeOfShootingModifier> GetShootingModifierFirstArray()
    {
        return shootingModifierFirstArray;
    }

    // Добавляет модификатор в список первого этапа
    public virtual void AddShootingModifierFirst(TypeOfShootingModifier modifier)
    {
        shootingModifierFirstArray.Add(modifier);
        log.Debug($"Добавлен модификатор первого этапа: {modifier}");
    }

    // Удаляет модификатор из списка первого этапа
    public virtual void RemoveShootingModifierFirst(TypeOfShootingModifier modifier)
    {
        if (shootingModifierFirstArray.Remove(modifier))
        {
            log.Debug($"Удалён модификатор первого этапа: {modifier}");
        }
        else
        {
            log.Debug($"Модификатор первого этапа {modifier} не найден для удаления.");
        }
    }

    // Возвращает список модификаторов стрельбы для второго этапа
    public virtual List<TypeOfShootingModifier> GetShootingModifierSecondArray()
    {
        return shootingModifierSecondArray;
    }

    // Добавляет модификатор в список второго этапа
    public virtual void AddShootingModifierSecond(TypeOfShootingModifier modifier)
    {
        shootingModifierSecondArray.Add(modifier);
        log.Debug($"Добавлен модификатор второго этапа: {modifier}");
    }

    // Удаляет модификатор из списка второго этапа
    public virtual void RemoveShootingModifierSecond(TypeOfShootingModifier modifier)
    {
        if (shootingModifierSecondArray.Remove(modifier))
        {
            log.Debug($"Удалён модификатор второго этапа: {modifier}");
        }
        else
        {
            log.Debug($"Модификатор второго этапа {modifier} не найден для удаления.");
        }
    }

    // Возвращает список модификаторов стрельбы для третьего этапа
    public virtual List<TypeOfShootingModifier> GetShootingModifierThirdArray()
    {
        return shootingModifierThirdArray;
    }

    // Добавляет модификатор в список третьего этапа
    public virtual void AddShootingModifierThird(TypeOfShootingModifier modifier)
    {
        shootingModifierThirdArray.Add(modifier);
        log.Debug($"Добавлен модификатор третьего этапа: {modifier}");
    }

    // Удаляет модификатор из списка третьего этапа
    public virtual void RemoveShootingModifierThird(TypeOfShootingModifier modifier)
    {
        if (shootingModifierThirdArray.Remove(modifier))
        {
            log.Debug($"Удалён модификатор третьего этапа: {modifier}");
        }
        else
        {
            log.Debug($"Модификатор третьего этапа {modifier} не найден для удаления.");
        }
    }
}