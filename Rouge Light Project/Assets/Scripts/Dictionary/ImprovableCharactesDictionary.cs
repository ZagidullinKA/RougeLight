using log4net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class ImprovableCharactesDictionary
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(ImprovableCharactesDictionary));

    private static readonly Dictionary<string, ItemImprovableCharactes> itemsImprovableCharactes = new();

    // Однократная инициализация справочника при обращении к любому из методов класса
    static ImprovableCharactesDictionary()
    {
        InitializeDictionary();
    }

    private static void InitializeDictionary()
    {
        itemsImprovableCharactes.Clear();

        // Добавляем улучшаемые характеристики из DictionaryCharacters
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            if (item.Upgradable)
            {
                string code = item.Code.ToString(); // Конвертируем CharacterStatCode в строку
                itemsImprovableCharactes.Add(
                    code,
                    CreateItem(code, item.NameRu, true, 0, item.BaseAmount, null, null, null, null));
            }
        }

        // Добавляем улучшаемые DoT-эффекты из DotsDictionary
        foreach (var dot in DotsDictionary.GetAllDots())
        {
            if (dot.Upgradable)
            {
                ValidationValue.ValidateStringNotNullOrEmpty(
                    (dot.NameRu, nameof(dot.NameRu)));

                ValidationValue.ValidateIntNotNull(
                    (dot.BaseDotDmg, nameof(dot.BaseDotDmg)),
                    (dot.BaseDotDuration, nameof(dot.BaseDotDuration)));

                string code = dot.Code.ToString(); // Конвертируем DotCode в строку
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

    public static List<ItemImprovableCharactes> GetListItemImprovableCharactesAndDots()
    {
        return new List<ItemImprovableCharactes>(itemsImprovableCharactes.Values);
    }

    public static ItemImprovableCharactes GetImprovableCharacteristicOrDot(string code)
    {
        return itemsImprovableCharactes.TryGetValue(code, out var item) ? item : null;
    }

    public static List<ItemImprovableCharactes> GetAllImprovableCharacteristicsAndDots()
    {
        return GetListItemImprovableCharactesAndDots();
    }

    public static List<ItemImprovableCharactes> GetAllImprovableDots()
    {
        return itemsImprovableCharactes.Values
            .Where(item => !item.Type) // false = DoT-эффект
            .ToList();
    }

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
