using System.Collections.Generic;

public static class DictionaryCharacters
{
    public static readonly Dictionary<string, ItemCharacter> itemCharacters = new()
    {
        { "maxHP", new ItemCharacter("maxHP", "максимальное ХП", true, 10, 1, 1) },
        { "dmg", new ItemCharacter("dmg", "дамаг", true, 1, 1, 2) },
        { "atkSpeed", new ItemCharacter("atkSpeed", "Скорость атаки", true, 10, 5, 3) },
        { "moveSpeed", new ItemCharacter("moveSpeed", "Скорость передвижения", true, 10, 5, 4) },
        { "luck", new ItemCharacter("luck", "Удача", false, 2, 1, 5) },
        { "critChance", new ItemCharacter("critChance", "Шанс крита", true, 0, 10, 6) },
        { "evadeChace", new ItemCharacter("evadeChace", "Шанс уворота", true, 0, 7, 7) },
        { "armor", new ItemCharacter("armor", "броня", true, 0, 2, 8) },
        { "debuffResist", new ItemCharacter("debuffResist", "сопротивление дебафам", true, 0, 3, 9) },
        { "Vampire", new ItemCharacter("Vampire", "Вампирка", true, 0, 1, 10) },
        { "hpFromDropRestore", new ItemCharacter("hpFromDropRestore", "Кол-во ХП при поднятии хилки", true, 10, 1, 11) },
        { "dropRadius", new ItemCharacter("dropRadius", "Радиус подбора", true, 10, 1, 12) }
    };
}