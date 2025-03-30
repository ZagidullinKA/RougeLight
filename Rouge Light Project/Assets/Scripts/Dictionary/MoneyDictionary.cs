using log4net;
using System.Collections.Generic;

public static class MoneyDictionary
{
    private static readonly ILog log = LogManager.GetLogger(typeof(MoneyDictionary));
    private static readonly Dictionary<MoneyCode, ItemMoneyDictionary> itemsMoneyDictionary = new()
    {
        // Формат: { Код, new ItemMoneyDictionary(Код, Количество) }
        { MoneyCode.Money, CreateMoneyItem(MoneyCode.Money, 12) }
    };

    // Вспомогательный метод для создания ItemMoneyDictionary
    private static ItemMoneyDictionary CreateMoneyItem(MoneyCode code, int amount)
    {
        return new ItemMoneyDictionary(code, amount);
    }

    public static ItemMoneyDictionary GetItemMoneyDictionaryOfCode(MoneyCode code)
    {
        return itemsMoneyDictionary.TryGetValue(code, out var item) ? item : null;
    }

    public static List<ItemMoneyDictionary> GetItemsMoneyDictionary()
    {
        return new List<ItemMoneyDictionary>(itemsMoneyDictionary.Values);
    }

    public static void IncreaseAmountItemMoneyDictionaryOfCode(MoneyCode code, int value)
    {
        if (itemsMoneyDictionary.TryGetValue(code, out var item))
        {
            item.Amount += value;
            log.Debug($"Количество {code} увеличено на {value}, теперь: {item.Amount}");
        }
        else
        {
            log.Warn($"Валюта с кодом {code} не найдена в справочнике.");
        }
    }
}