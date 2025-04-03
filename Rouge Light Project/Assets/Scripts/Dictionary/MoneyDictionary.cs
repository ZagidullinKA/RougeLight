using log4net;
using System.Collections.Generic;

// Статический класс MoneyDictionary - справочник денежных единиц в игре
// Содержит информацию о всех типах денег и их количестве
public static class MoneyDictionary
{
    // Логгер для записи событий и ошибок
    private static readonly ILog log = LogManager.GetLogger(typeof(MoneyDictionary));

    // Словарь для хранения всех денежных единиц в игре
    // Ключ - код денежной единицы, значение - объект с данными
    private static readonly Dictionary<MoneyCode, ItemMoneyDictionary> itemsMoneyDictionary = new()
    {
        // Формат: { Код, new ItemMoneyDictionary(Код, Количество) }
        { MoneyCode.Money, CreateMoneyItem(MoneyCode.Money, 12) } // Базовая валюта с начальным количеством 12
    };

    // Вспомогательный метод для создания ItemMoneyDictionary
    // Позволяет удобно создавать новые записи о денежных единицах
    private static ItemMoneyDictionary CreateMoneyItem(MoneyCode code, int amount)
    {
        return new ItemMoneyDictionary(code, amount);
    }

    // Метод для получения данных о конкретной денежной единице по ее коду
    public static ItemMoneyDictionary GetItemMoneyDictionaryOfCode(MoneyCode code)
    {
        // Пытаемся получить значение из словаря, возвращаем null если не найдено
        return itemsMoneyDictionary.TryGetValue(code, out var item) ? item : null;
    }

    // Метод для получения списка всех денежных единиц в системе
    public static List<ItemMoneyDictionary> GetItemsMoneyDictionary()
    {
        // Создаем новый список на основе значений словаря
        return new List<ItemMoneyDictionary>(itemsMoneyDictionary.Values);
    }

    // Метод для увеличения количества конкретной денежной единицы
    public static void IncreaseAmountItemMoneyDictionaryOfCode(MoneyCode code, int value)
    {
        // Проверяем наличие денежной единицы в словаре
        if (itemsMoneyDictionary.TryGetValue(code, out var item))
        {
            // Увеличиваем количество
            item.Amount += value;
            // Логируем операцию
            log.Debug($"Количество {code} увеличено на {value}, теперь: {item.Amount}");
        }
        else
        {
            // Логируем предупреждение, если валюта не найдена
            log.Warn($"Валюта с кодом {code} не найдена в справочнике.");
        }
    }
}