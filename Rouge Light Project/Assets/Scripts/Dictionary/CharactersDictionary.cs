using System.Collections.Generic;

public class DictionaryCharacters
{
    private static readonly List<ItemCharacter> itemCharacters = new()
    {
        //структура ItemCharacter: string code, string nameRu, bool upgradable, int upgradeX, int baseAmount, int price, bool isEnemyAvaliable
        new ItemCharacter("maxHP",              "максимальное ХП",              true,   10, 1,  1,  true) ,
        new ItemCharacter("dmg",                "дамаг",                        true,   1,  5,  2,  true),
        new ItemCharacter("atkSpeed",           "Скорость атаки",               true,   10, 5,  3,  true),
        new ItemCharacter("moveSpeed",          "Скорость передвижения",        true,   10, 5,  4,  true),
        new ItemCharacter("luck",               "Удача",                        false,  2,  1,  5,  true),
        new ItemCharacter("critChance",         "Шанс крита",                   true,   0,  10, 6,  true),
        new ItemCharacter("evadeChace",         "Шанс уворота",                 true,   0,  7,  7,  true),
        new ItemCharacter("armor",              "броня",                        true,   0,  2,  8,  true), 
        new ItemCharacter("debuffResist",       "сопротивление дебафам",        true,   0,  3,  9,  true),
        new ItemCharacter("Vampire",            "Вампирка",                     true,   0,  1,  10, true),
        new ItemCharacter("hpFromDropRestore",  "Кол-во ХП при поднятии хилки", true,   10, 1,  11, true),
        new ItemCharacter("dropRadius",         "Радиус подбора",               true,   10, 1,  12, true),
        new ItemCharacter("bulletflySpeed",     "Скорость полета пули",         true,   10, 1,  12, true),
        new ItemCharacter("bulletTimeAlive",    "Время жизни пули",             true,   10, 1,  12, true)
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