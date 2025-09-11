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
    [SerializeField] protected int dmg = 0;                      // Урон атаки
    [SerializeField] protected int meleeDmg = 0;                 // Урон ближнего боя
    [SerializeField] protected float atkSpeed = 0f;              // Скорость атаки
    [SerializeField] protected int moveSpeed = 0;                // Скорость перемещения
    [SerializeField] protected int critChance = 0;               // Шанс критического удара
    [SerializeField] protected int evadeChance = 0;              // Шанс уклонения
    [SerializeField] protected int armor = 0;                    // Броня
    [SerializeField] protected int debuffResist = 0;             // Сопротивление дебаффам
    [SerializeField] protected int vampire = 0;                  // Вампиризм (лечение от урона)
    [SerializeField] protected int hpFromDropRestore = 0;        // Восстанавливаемое HP от подбираемого предмета
    [SerializeField] protected int bulletFlySpeed = 0;           // Скорость полета пули
    [SerializeField] protected int bulletTimeAlive = 0;          // Время жизни пули
    [SerializeField] protected int actualHP = 0;                 // Текущее здоровье персонажа (изменяется в игре)
    [SerializeField] protected bool isCanDie = true;             // Может ли персонаж умереть (изменяется в игре)
    [SerializeField] protected bool isEnemy = false;             // Является ли персонаж врагом (изменяется в игре)
    [SerializeField] protected float rotateSpeed = 0;             // Скорость поворота персонажа (изменяется в игре)
    [SerializeField] protected List<RecievedDotEffect> recievedDotsArray = new List<RecievedDotEffect>(); // список получаемых DoT-эффектов
    [SerializeField] protected List<UsableDotEffect> usableDotsArray = new List<UsableDotEffect>();       // список используемых DoT-эффектов
    [SerializeField] protected List<ShotPoint> shotPointsArray = new List<ShotPoint>();              // список точек стрельбы
    [SerializeField] protected List<TypeOfShootingModifier> shootingModifierSecondArray = new List<TypeOfShootingModifier>();             // список модификаторов стрельбы 2 слот
    [SerializeField] protected List<TypeOfShootingModifier> shootingModifierThirdArray = new List<TypeOfShootingModifier>();              // список модификаторов стрельбы 3 слот

    // Свойства get и set для основных характеристик
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

    // Установка значения конкретной характеристики по названию в игре и изменению в числовом виде
    public virtual void SetStat(string statName, float? value)
    {
        if (value == null)
        {
            log.Error($"Значение для {statName} не указано!");
            return;
        }

        // Преобразование statName из enum CharacterStatCode в строку (camelCase)
        if (Enum.TryParse<CharacterStatCode>(statName, ignoreCase: true, out CharacterStatCode statCode))
        {
            // Преобразуем CharacterStatCode в строку (camelCase)
            string fieldName = char.ToLower(statCode.ToString()[0]) + statCode.ToString().Substring(1);
            // Находим поле, соответствующее fieldName
            var field = GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                var currentValue = Convert.ToSingle(field.GetValue(this)); // Преобразуем field в float для дальнейших вычислений
                var newValue = currentValue + value.Value;
                field.SetValue(this, Convert.ChangeType(newValue, field.FieldType)); // Устанавливаем значение нового поля
                log.Debug($"SetStat: {statName} изменился на {newValue}");
            }
            else
            {
                log.Warn($"Поле не найдено: {statName}");
            }
        }
        else
        {
            log.Warn($"Не найдено соответствие '{statName}' в CharacterStatCode.");
        }
    }

    // Получение значения конкретной характеристики по названию в игре и изменению в числовом виде
    public virtual float? GetStat(string statName)
    {

        // Находим поле, соответствующее fieldName
        var field = GetType().GetField(statName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field == null)
        {
            log.Debug($"Поле не найдено: {statName}"); // проверяем существование поля
            return null;
        }

        return (float?) field.GetValue(this);
    }

    // Добавление получаемого DoT-эффекта в список
    public virtual void SetRecievedDots(RecievedDotEffect dotEffect)
    {
        recievedDotsArray.Add(dotEffect);
        log.Debug($"Добавлен RecievedDotEffect: {dotEffect.Code}"); // проверяем добавление
    }

    // Удаляем из списка recievedDotsArray все элементы, содержащиеся в переданном списке
    public virtual void RemoveRecievedDots(List<RecievedDotEffect> removeRecievedDotsArray)
    {
        int removedCount = recievedDotsArray.RemoveAll(item => removeRecievedDotsArray.Contains(item));
        if (removedCount > 0)
        {
            log.Debug($"Удалено {removedCount} RecievedDotEffect из списка."); // проверяем успешность удаления элементов
        }
        else
        {
            log.Debug("Не найдено RecievedDotEffect для удаления."); // сообщение, если элемент не найден
        }
    }

    // Возвращает список получаемых DoT-эффектов
    public virtual List<RecievedDotEffect> GetRecievedDots()
    {
        return recievedDotsArray;
    }

    // Добавление используемого DoT-эффекта в список
    public virtual void SetUsableDots(UsableDotEffect usableEffect)
    {
        usableDotsArray.Add(usableEffect);
        log.Debug($"Добавлен UsableDotEffect: {usableEffect.Code}"); // проверяем добавление
    }

    // Возвращает список используемых DoT-эффектов
    public virtual List<UsableDotEffect> GetUsableDots()
    {
        return usableDotsArray;
    }

    // Инициализация характеристик персонажа из справочника
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

    // Возвращает список точек стрельбы
    public virtual List<ShotPoint> GetShotPointsArray()
    {
        return shotPointsArray;
    }

    // Добавление точки стрельбы в список
    public virtual void AddShotPoint(ShotPoint shotPoint)
    {
        shotPointsArray.Add(shotPoint);
        log.Debug($"Добавлена точка стрельбы: угол={shotPoint.angle}°, направление={shotPoint.direction}, активна={shotPoint.isActive}");
    }

    // Удаление точки стрельбы из списка
    public virtual void RemoveShotPoint(ShotPoint shotPoint)
    {
        if (shotPointsArray.Remove(shotPoint))
        {
            log.Debug($"Удалена точка стрельбы: угол={shotPoint.angle}°");
        }
        else
        {
            log.Debug($"Точка стрельбы с углом {shotPoint.angle}° не найдена для удаления.");
        }
    }

    // Очистка всех точек стрельбы
    public virtual void ClearShotPoints()
    {
        shotPointsArray.Clear();
        log.Debug("Очищены все точки стрельбы");
    }

    // Установка точек стрельбы по умолчанию (одна точка вперед)
    public virtual void SetDefaultShotPoints()
    {
        shotPointsArray.Clear();
        shotPointsArray.Add(new ShotPoint(0f, Vector2.up, true));
        log.Debug("Установлены точки стрельбы по умолчанию");
    }

    public virtual void SetDefaultShotPointsByMobs()
    {
        shotPointsArray.Clear();
        shotPointsArray.Add(new ShotPoint(0f, Vector2.right, true));
        log.Debug("Установлены точки стрельбы по умолчанию");
    }

    // Устанавливает новый массив точек стрельбы
    public virtual void SetShotPointsArray(List<ShotPoint> newShotPoints)
    {
        shotPointsArray.Clear();
        shotPointsArray.AddRange(newShotPoints);
        log.Debug($"Установлен новый массив точек стрельбы: {newShotPoints.Count} точек");
    }

    // Возвращает список модификаторов стрельбы для второго слота
    public virtual List<TypeOfShootingModifier> GetShootingModifierSecondArray()
    {
        return shootingModifierSecondArray;
    }

    // Добавление модификатора во второй слот
    public virtual void AddShootingModifierSecond(TypeOfShootingModifier modifier)
    {
        shootingModifierSecondArray.Add(modifier);
        log.Debug($"Добавлен модификатор второго слота: {modifier}");
    }

    // Удаление модификатора из второго слота
    public virtual void RemoveShootingModifierSecond(TypeOfShootingModifier modifier)
    {
        if (shootingModifierSecondArray.Remove(modifier))
        {
            log.Debug($"Удален модификатор второго слота: {modifier}");
        }
        else
        {
            log.Debug($"Модификатор второго слота {modifier} не найден для удаления.");
        }
    }

    // Возвращает список модификаторов стрельбы для третьего слота
    public virtual List<TypeOfShootingModifier> GetShootingModifierThirdArray()
    {
        return shootingModifierThirdArray;
    }

    // Добавление модификатора в третий слот
    public virtual void AddShootingModifierThird(TypeOfShootingModifier modifier)
    {
        shootingModifierThirdArray.Add(modifier);
        log.Debug($"Добавлен модификатор третьего слота: {modifier}");
    }

    // Удаление модификатора из третьего слота
    public virtual void RemoveShootingModifierThird(TypeOfShootingModifier modifier)
    {
        if (shootingModifierThirdArray.Remove(modifier))
        {
            log.Debug($"Удален модификатор третьего слота: {modifier}");
        }
        else
        {
            log.Debug($"Модификатор третьего слота {modifier} не найден для удаления.");
        }
    }
}
