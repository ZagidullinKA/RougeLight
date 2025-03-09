using log4net;
using NUnit.Framework;
using System.Collections.Generic;

public static class MoneyDictionary
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(MoneyDictionary));

    private static readonly List<ItemMoneyDictionary> ItemsMoneyDictionary = new() {
        new ItemMoneyDictionary("money", 12)
    };


    public static ItemMoneyDictionary GetItemMoneyDictionaryOfCode(string code)
    {
        return ItemsMoneyDictionary.Find(x => x.Code == code);
    }

    public static List<ItemMoneyDictionary> GetItemsMoneyDictionary()
    {
        return ItemsMoneyDictionary;
    }

    public static void increaseAmountItemMoneyDictionaryOfCode(string code, int value)
    {
        for (int i = 0; i < ItemsMoneyDictionary.Count; i++)
        {
            if (ItemsMoneyDictionary[i].Code == code)
            {
                ItemsMoneyDictionary[i].Amount = ItemsMoneyDictionary[i].Amount + value;
                break;
            }
        }
    }
}
