
using System.Collections.Generic;

public class EnemyTypesDictionary
{
    private static readonly List<ItemEnemyTypesDictionary> itemEnemys = new()
    {
        //структура ItemCharacter:
        // string code, string nameRu, int maxHP, int dmg, int atkSpeed, 
        // int moveSpeed, int critChance, int evadeChance, int armor, int debuffResist, int vampire,
        // int bulletflySpeed, int bulletTimeAlive, string typeOfAttack
        new ItemEnemyTypesDictionary("UnitTest", "Тестовый юнит", 100, 1, 1, 1, 10, 5, 0, 0, 0, 1, 3, TypeOfEnemyAttack.TYPE_SHOOT)
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
