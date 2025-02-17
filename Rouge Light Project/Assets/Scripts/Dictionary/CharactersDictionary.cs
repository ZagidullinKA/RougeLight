using System.Collections.Generic;

public class DictionaryCharacters
{
    private static readonly List<ItemCharacter> itemCharacters = new()
    {
        new ItemCharacter("maxHP", "максимальное ХП", true, 10, 1, 1) ,
        new ItemCharacter("dmg", "дамаг", true, 1, 1, 2),
        new ItemCharacter("atkSpeed", "Скорость атаки", true, 10, 5, 3),
        new ItemCharacter("moveSpeed", "Скорость передвижения", true, 10, 5, 4),
        new ItemCharacter("luck", "Удача", false, 2, 1, 5),
        new ItemCharacter("critChance", "Шанс крита", true, 0, 10, 6),
        new ItemCharacter("evadeChace", "Шанс уворота", true, 0, 7, 7),
        new ItemCharacter("armor", "броня", true, 0, 2, 8),
        new ItemCharacter("debuffResist", "сопротивление дебафам", true, 0, 3, 9),
        new ItemCharacter("Vampire", "Вампирка", true, 0, 1, 10),
        new ItemCharacter("hpFromDropRestore", "Кол-во ХП при поднятии хилки", true, 10, 1, 11),
        new ItemCharacter("dropRadius", "Радиус подбора", true, 10, 1, 12) 
    };

    public static ItemCharacter GetCharacteristic(string code)
    {
        return itemCharacters.Find(x => x.Code == code);
    }

    public static List<ItemCharacter> GetAllCharacteristics()
    {
        return itemCharacters;
    }
}