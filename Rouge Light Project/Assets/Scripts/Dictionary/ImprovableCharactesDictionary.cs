using log4net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

// Статический класс для работы с улучшаемыми характеристиками и эффектами (DoT)
public static class ImprovableCharactesDictionary
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(ImprovableCharactesDictionary));

    // Словарь для хранения всех улучшаемых характеристик и эффектов
    // Ключ - строковый код, значение - объект с данными улучшаемой характеристики/эффекта
    private static readonly Dictionary<string, ItemImprovableCharactes> itemsImprovableCharactes = new();

    // Статический конструктор - выполняется один раз при первом обращении к классу
    static ImprovableCharactesDictionary()
    {
        InitializeDictionary(); // Инициализация словаря при первом использовании класса
    }

    // Метод инициализации словаря характеристик и эффектов
    private static void InitializeDictionary()
    {
        itemsImprovableCharactes.Clear(); // Очищаем словарь перед заполнением

        // Добавляем улучшаемые характеристики из DictionaryCharacters
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            if (item.Upgradable) // Проверяем, можно ли улучшать характеристику
            {
                string code = item.Code.ToString(); // Конвертируем CharacterStatCode в строку
                // Создаем и добавляем запись в словарь
                itemsImprovableCharactes.Add(
                    code,
                    CreateItem(code, item.NameRu, true, 0, item.BaseAmount, null, null, null, null));
            }
        }

        // Добавляем улучшаемые DoT-эффекты из DotsDictionary
        foreach (var dot in DotsDictionary.GetAllDots())
        {
            if (dot.Upgradable) // Проверяем, можно ли улучшать эффект
            {
                // Валидация обязательных полей
                ValidationValue.ValidateStringNotNullOrEmpty(
                    (dot.NameRu, nameof(dot.NameRu)));

                ValidationValue.ValidateIntNotNull(
                    (dot.BaseDotDmg, nameof(dot.BaseDotDmg)),
                    (dot.BaseDotDuration, nameof(dot.BaseDotDuration)));

                string code = dot.Code.ToString(); // Конвертируем DotCode в строку
                // Создаем и добавляем запись в словарь
                itemsImprovableCharactes.Add(
                    code,
                    CreateItem(code, dot.NameRu, false, null, null, 0, 0, dot.BaseDotDmg, dot.BaseDotDuration));
            }
        }
    }

    // Вспомогательный метод для создания ItemImprovableCharactes
    private static ItemImprovableCharactes CreateItem(string code, string nameRu, bool type,
        int? upgradeAmount, float? finalValue, int? dmgUpgradeAmount,
        int? durationUpgradeAmount, int? finalDotDmg, int? finalDotDur)
    {
        return new ItemImprovableCharactes(code, nameRu, type, upgradeAmount, finalValue,
            dmgUpgradeAmount, durationUpgradeAmount, finalDotDmg, finalDotDur);
    }

    // Получение списка всех улучшаемых характеристик и эффектов
    public static List<ItemImprovableCharactes> GetListItemImprovableCharactesAndDots()
    {
        return new List<ItemImprovableCharactes>(itemsImprovableCharactes.Values);
    }

    // Получение конкретной характеристики/эффекта по коду
    public static ItemImprovableCharactes GetImprovableCharacteristicOrDot(string code)
    {
        return itemsImprovableCharactes.TryGetValue(code, out var item) ? item : null;
    }

    // Алиас для GetListItemImprovableCharactesAndDots()
    public static List<ItemImprovableCharactes> GetAllImprovableCharacteristicsAndDots()
    {
        return GetListItemImprovableCharactesAndDots();
    }

    // Получение только улучшаемых DoT-эффектов (где Type = false)
    public static List<ItemImprovableCharactes> GetAllImprovableDots()
    {
        return itemsImprovableCharactes.Values
            .Where(item => !item.Type) // false = DoT-эффект
            .ToList();
    }

    // Получение финального значения урона для DoT-эффекта по коду
    public static int GetFinalDotDmgOfCode(string code)
    {
        var item = itemsImprovableCharactes.TryGetValue(code, out var foundItem) ? foundItem : null;
        if (item == null || item.FinalDotDmg == null)
        {
            log.Error($"Не верный code или поле FinalDotDmg не заполнено, code = {code}");
            return 0;
        }
        return item.FinalDotDmg.Value;
    }

    // Получение финальной длительности для DoT-эффекта по коду
    public static int GetFinalDotDurOfCode(string code)
    {
        var item = itemsImprovableCharactes.TryGetValue(code, out var foundItem) ? foundItem : null;
        if (item == null || item.FinalDotDur == null)
        {
            log.Error($"Не верный code или поле FinalDotDur не заполнено, code = {code}");
            return 0;
        }
        return item.FinalDotDur.Value;
    }
}