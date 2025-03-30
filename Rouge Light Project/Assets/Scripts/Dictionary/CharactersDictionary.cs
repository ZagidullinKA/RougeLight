using System.Collections.Generic;

public class DictionaryCharacters
{
    private static readonly Dictionary<CharacterStatCode, ItemCharacter> itemCharacters = new()
    {
        // Формат: { Код, new ItemCharacter(    Код,                                            Название,                       Улучшаемость, Шаг улучшения, Базовое значение, Цена, Доступно врагам) }
        { CharacterStatCode.MaxHP,              CreateStat(CharacterStatCode.MaxHP,             "максимальное ХП",              true,   10,     1f,     1,  true)   },
        { CharacterStatCode.Dmg,                CreateStat(CharacterStatCode.Dmg,               "дамаг",                        true,   1,      1f,     2,  true)   },
        { CharacterStatCode.AtkSpeed,           CreateStat(CharacterStatCode.AtkSpeed,          "Скорость атаки",               true,   10,     5f,     3,  true)   },
        { CharacterStatCode.MoveSpeed,          CreateStat(CharacterStatCode.MoveSpeed,         "Скорость передвижения",        true,   10,     5f,     4,  true)   },
        { CharacterStatCode.CritChance,         CreateStat(CharacterStatCode.CritChance,        "Шанс крита",                   true,   0,      10f,    6,  true)   },
        { CharacterStatCode.EvadeChance,        CreateStat(CharacterStatCode.EvadeChance,       "Шанс уворота",                 true,   0,      7f,     7,  true)   },
        { CharacterStatCode.Armor,              CreateStat(CharacterStatCode.Armor,             "броня",                        true,   0,      2f,     8,  true)   },
        { CharacterStatCode.DebuffResist,       CreateStat(CharacterStatCode.DebuffResist,      "сопротивление дебафам",        true,   0,      3f,     9,  true)   },
        { CharacterStatCode.Vampire,            CreateStat(CharacterStatCode.Vampire,           "Вампирка",                     true,   0,      1f,     10, true)   },
        { CharacterStatCode.HpFromDropRestore,  CreateStat(CharacterStatCode.HpFromDropRestore, "Кол-во ХП при поднятии хилки", true,   10,     1f,     11, true)   },
        { CharacterStatCode.BulletFlySpeed,     CreateStat(CharacterStatCode.BulletFlySpeed,    "Скорость полета пули",         true,   10,     2f,     12, true)   },
        { CharacterStatCode.BulletTimeAlive,    CreateStat(CharacterStatCode.BulletTimeAlive,   "Время жизни пули",             true,   10,     2f,     12, true)   },
        { CharacterStatCode.RotateSpeed,        CreateStat(CharacterStatCode.RotateSpeed,       "Скорость поворота",            true,   10,     2f,     12, true)   },
        { CharacterStatCode.DropRadius,         CreateStat(CharacterStatCode.DropRadius,        "Радиус подбора",               true,   10,     1f,     12, false)  },
        { CharacterStatCode.Luck,               CreateStat(CharacterStatCode.Luck,              "Удача",                        false,  2,      2f,     5,  false)  },
    };

    // Вспомогательный метод для создания ItemCharacter с читаемым форматом
    private static ItemCharacter CreateStat(CharacterStatCode code, string nameRu, bool upgradable, int upgradeX, float baseAmount, int price, bool isEnemyAvailable)
    {
        return new ItemCharacter(code, nameRu, upgradable, upgradeX, baseAmount, price, isEnemyAvailable);
    }

    public static ItemCharacter GetCharacteristic(CharacterStatCode code)
    {
        return itemCharacters.TryGetValue(code, out var item) ? item : null;
    }

    public static List<ItemCharacter> GetAllCharacteristics()
    {
        return new List<ItemCharacter>(itemCharacters.Values);
    }
}