using log4net;
using System.Collections.Generic;
using System.Linq;

public static class ShootingModifiersDictionary
{
    // Логгер для отладки ошибок и информационных сообщений
    private static readonly ILog log = LogManager.GetLogger(typeof(ShootingModifiersDictionary));

    // Словарь модификаторов стрельбы: ключ - ShootingModifierCode, значение - ItemShootingModifiersDictionary
    private static readonly Dictionary<TypeOfShootingModifier, ItemShootingModifiersDictionary> itemsShootingModifiersDictionary = new()
    {
        // Формат: { ShootingModifierCode, new ItemShootingModifiersDictionary(code,                                lvl, count, type,      accuracy) }
        { TypeOfShootingModifier.BaseShootingFirst,   CreateShootingModifier(TypeOfShootingModifier.BaseShootingFirst,   1,   1, "base",       null) },
        { TypeOfShootingModifier.BaseShootingSecond,  CreateShootingModifier(TypeOfShootingModifier.BaseShootingSecond,  2,   1, "base",       null) },
        { TypeOfShootingModifier.BaseShootingThird,   CreateShootingModifier(TypeOfShootingModifier.BaseShootingThird,   3,   1, "base",       1f)   },
        { TypeOfShootingModifier.Circle6,             CreateShootingModifier(TypeOfShootingModifier.Circle6,             1,   6, "circle",     null) },
        { TypeOfShootingModifier.SemiCircle3,         CreateShootingModifier(TypeOfShootingModifier.SemiCircle3,         1,   3, "semiCircle", null) },
        { TypeOfShootingModifier.Burst3,              CreateShootingModifier(TypeOfShootingModifier.Burst3,              2,   3, "burst",      null) },
        { TypeOfShootingModifier.Buckshot3,           CreateShootingModifier(TypeOfShootingModifier.Buckshot3,           3,   3, "buckshot",   0.6f) }
    };

    // Вспомогательный метод для создания ItemShootingModifiersDictionary
    private static ItemShootingModifiersDictionary CreateShootingModifier(TypeOfShootingModifier code, int lvl, int count, string type, float? accuracy)
    {
        return new ItemShootingModifiersDictionary(code, lvl, count, type, accuracy ?? 0f);
    }

    // Получение модификатора по коду
    public static ItemShootingModifiersDictionary GetItemShootingModifierOfCode(TypeOfShootingModifier code)
    {
        return itemsShootingModifiersDictionary.TryGetValue(code, out var item) ? item : null;
    }

    // Получение списка всех модификаторов
    public static List<ItemShootingModifiersDictionary> GetItemsShootingModifiersDictionary()
    {
        return new List<ItemShootingModifiersDictionary>(itemsShootingModifiersDictionary.Values);
    }

    // Получение всех модификаторов по указанному уровню
    public static List<ItemShootingModifiersDictionary> GetListModifiersByLevel(int lvl)
    {
        return itemsShootingModifiersDictionary.Values
            .Where(item => item.Lvl == lvl)
            .ToList();
    }
}