
using System.Collections.Generic;

public class EnemyTypesDictionary
{
    private static readonly List<ItemEnemyTypesDictionary> itemEnemys = new()
    {
        new ItemEnemyTypesDictionary(
        "UnitTest",                         //string code,
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
        false                               //bool isBoss
        )
    };

    public static ItemEnemyTypesDictionary GetCharacteristic(string code)
    {
        return itemEnemys.Find(x => x.Code == code);
    }

    public static List<ItemEnemyTypesDictionary> GetAllCharacteristics()
    {
        return itemEnemys;
    }
}
