using log4net;
using System.Collections.Generic;

public class MoneyDictionary
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(MoneyDictionary));

    private static readonly List<ItemMoneyDictionary> ItemsMoneyDictionary = new() {
        new ItemMoneyDictionary("money", 0)
    };


    public static ItemMoneyDictionary GetImprovableCharacteristic(string code)
    {
        return ItemsMoneyDictionary.Find(x => x.Code == code);
    }

    public static List<ItemMoneyDictionary> GetAllImprovableCharacteristics()
    {
        return ItemsMoneyDictionary;
    }

}
