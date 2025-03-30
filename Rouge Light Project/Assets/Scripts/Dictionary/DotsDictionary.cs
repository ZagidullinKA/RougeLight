
using System.Collections.Generic;

public static class DotsDictionary
{
    // Формат: 
    private static readonly Dictionary<DotCode, ItemDot> ITEM_DOTS = new()
    {   
        // { Код, new ItemDot }     Код,            Название, Шаг улучшения урона, Шаг улучшения длительности, Базовый урон, Базовая длительность, Цель, Тип, Улучшаемость
        { DotCode.Fire,   CreateDot(DotCode.Fire,   "Огонь",    1, 1, 1, 5, CharacterStatCode.ActualHP, TypeOfDots.TYPE_FIXED, true) },
        { DotCode.Poison, CreateDot(DotCode.Poison, "Яд",       2, 2, 2, 2, CharacterStatCode.ActualHP, TypeOfDots.TYPE_FIXED, true) }
    };

    // Вспомогательный метод для создания ItemDot с читаемым форматом
    private static ItemDot CreateDot(DotCode code, string nameRu, int upgradeDotDmgX, int upgradeDotDurX,
                                     int baseDotDmg, int baseDotDuration, CharacterStatCode affectedChar,
                                     TypeOfDots type, bool upgradable)
    {
        return new ItemDot(code, nameRu, upgradeDotDmgX, upgradeDotDurX, baseDotDmg, baseDotDuration,
                           affectedChar, type, upgradable);
    }

    public static ItemDot GetDot(DotCode code)
    {
        return ITEM_DOTS.TryGetValue(code, out var item) ? item : null;
    }

    public static List<ItemDot> GetAllDots()
    {
        return new List<ItemDot>(ITEM_DOTS.Values);
    }
}
