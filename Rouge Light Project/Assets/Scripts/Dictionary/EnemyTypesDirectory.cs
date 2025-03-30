
using System.Collections.Generic;

public static class EnemyTypesDictionary
{
    private static readonly Dictionary<EnemyTypeCode, ItemEnemyTypesDictionary> itemEnemys = new()
    {
        { EnemyTypeCode.UnitTest, CreateEnemy(
            EnemyTypeCode.UnitTest,
            "Тестовый юнит",                    //string nameRu,
            200,                                //int maxHP,
            5,                                  //int dmg,
            1,                                  //float atkSpeed,
            1,                                  //int moveSpeed,
            10,                                 //int critChance,
            5,                                  //int evadeChance,
            0,                                  //int armor,
            0,                                  //int debuffResist,
            0,                                  //int vampire,
            2,                                  //int bulletFlySpeed,
            3,                                  //int bulletTimeAlive,
            10,                                 //int rotateSpeed,
            TypeOfEnemyAttack.TYPE_SHOOT,       //string typeOfAttack,
            0,                                  //int countOfDots,
            0,                                  //int countOfBulletModifiers,
            0,                                  //int countOfShootingModifiers,
            1,                                  //int meleeDmg,
            2,                                  //int deathPrice,
            false                               //bool isBoss) }
        )} 
    };

    // Вспомогательный метод для создания ItemEnemyTypesDictionary с читаемым форматом
    private static ItemEnemyTypesDictionary CreateEnemy(
        EnemyTypeCode code, string nameRu,
        int maxHP, int dmg, float atkSpeed, int moveSpeed,
        int critChance, int evadeChance, int armor, int debuffResist,
        int vampire, int bulletFlySpeed, int bulletTimeAlive, int rotateSpeed,
        TypeOfEnemyAttack typeOfAttack, int countOfDots,
        int countOfBulletModifiers, int countOfShootingModifiers,
        int meleeDmg, int deathPrice, bool isBoss)
    {
        return new ItemEnemyTypesDictionary(
            code, nameRu, maxHP, dmg, atkSpeed, moveSpeed,
            critChance, evadeChance, armor, debuffResist,
            vampire, bulletFlySpeed, bulletTimeAlive, rotateSpeed,
            typeOfAttack, countOfDots, countOfBulletModifiers,
            countOfShootingModifiers, meleeDmg, deathPrice, isBoss);
    }

    public static ItemEnemyTypesDictionary GetCharacteristic(EnemyTypeCode code)
    {
        return itemEnemys.TryGetValue(code, out var item) ? item : null;
    }

    public static List<ItemEnemyTypesDictionary> GetAllCharacteristics()
    {
        return new List<ItemEnemyTypesDictionary>(itemEnemys.Values);
    }
}
