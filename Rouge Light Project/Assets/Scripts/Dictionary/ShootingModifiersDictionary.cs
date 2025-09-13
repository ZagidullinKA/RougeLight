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
        // Формат: { ShootingModifierCode, new ItemShootingModifiersDictionary(code,                                lvl, count, type,      accuracy, range) }
        { TypeOfShootingModifier.BaseShootingFirst,   CreateShootingModifier(TypeOfShootingModifier.BaseShootingFirst,   1,   1, "base",       null, new int[] {0, 0}) },
        { TypeOfShootingModifier.BaseShootingSecond,  CreateShootingModifier(TypeOfShootingModifier.BaseShootingSecond,  2,   1, "base",       null, new int[] {0, 0}) },
        { TypeOfShootingModifier.BaseShootingThird,   CreateShootingModifier(TypeOfShootingModifier.BaseShootingThird,   3,   1, "base",       1f,  new int[] {0, 0}) },
        //{ TypeOfShootingModifier.Circle6,             CreateShootingModifier(TypeOfShootingModifier.Circle6,             1,   6, "circle",     null, new int[] {0, 360}) },
        { TypeOfShootingModifier.Sector3,             CreateShootingModifier(TypeOfShootingModifier.Sector3,             1,   3, "sector",     null, new int[] {15, 50}) },
        { TypeOfShootingModifier.Sector5,             CreateShootingModifier(TypeOfShootingModifier.Sector5,             1,   5, "sector",     null, new int[] {60, 110}) },
        { TypeOfShootingModifier.Sector2,             CreateShootingModifier(TypeOfShootingModifier.Sector2,             1,   2, "sector",     null, new int[] {150, 300}) },
        { TypeOfShootingModifier.Sector1,             CreateShootingModifier(TypeOfShootingModifier.Sector1,             1,   2, "sector",     null, new int[] {280, 20}) },
        { TypeOfShootingModifier.Sector6,             CreateShootingModifier(TypeOfShootingModifier.Sector6,             1,   2, "sector",     null, new int[] {0, 30}) },
        //{ TypeOfShootingModifier.SemiCircle3,         CreateShootingModifier(TypeOfShootingModifier.SemiCircle3,         1,   3, "semiCircle", null, new int[] {270, 90}) },
        { TypeOfShootingModifier.Burst3,              CreateShootingModifier(TypeOfShootingModifier.Burst3,              2,   3, "burst",      null, new int[] {0, 0}) },
        { TypeOfShootingModifier.Buckshot3,           CreateShootingModifier(TypeOfShootingModifier.Buckshot3,           3,   3, "buckshot",   0.1f, new int[] {0, 0}) }
    };

    // Вспомогательный метод для создания ItemShootingModifiersDictionary
    private static ItemShootingModifiersDictionary CreateShootingModifier(TypeOfShootingModifier code, int lvl, int count, string type, float? accuracy, int[] range)
    {
        return new ItemShootingModifiersDictionary(code, lvl, count, type, accuracy ?? 0f, range);
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

    // Метод, который выдает рандомный lvl от 1 до максимального в Dictionary
    public static int GetRandomLevel()
    {
        if (itemsShootingModifiersDictionary.Count == 0)
        {
            log.Error("Словарь модификаторов пуст!");
            return 1;
        }
        int maxLvl = itemsShootingModifiersDictionary.Values.Max(item => item.Lvl);
        return UnityEngine.Random.Range(1, maxLvl + 1); // верхняя граница не включается, поэтому +1
    }

    // Метод, который выдает рандомный код модификатора по уровню (исключая базовые модификаторы)
    public static string GetRandomModifierCodeByLevel(int lvl)
    {
        var modifiers = itemsShootingModifiersDictionary
            .Where(pair => pair.Value.Lvl == lvl && pair.Value.Type != "base")
            .Select(pair => pair.Key)
            .ToList();

        if (modifiers.Count == 0)
        {
            log.Warn($"Нет модификаторов для уровня {lvl} (исключая базовые)");
            return null;
        }

        int index = UnityEngine.Random.Range(0, modifiers.Count);
        return modifiers[index].ToString();
    }
}