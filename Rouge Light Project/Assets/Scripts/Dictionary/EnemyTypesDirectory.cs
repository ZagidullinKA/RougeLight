using System.Collections.Generic;

// Статический класс EnemyTypesDictionary - справочник типов врагов в игре
// Содержит параметры для каждого типа вражеских юнитов
public static class EnemyTypesDictionary
{
    // Словарь для хранения всех типов врагов
    // Ключ - тип врага (EnemyTypeCode), значение - параметры врага (ItemEnemyTypesDictionary)
    private static readonly Dictionary<EnemyTypeCode, ItemEnemyTypesDictionary> itemEnemys = new()
    {
        // Пример тестового юнита с базовыми параметрами
        { EnemyTypeCode.UnitTest, CreateEnemy(
            EnemyTypeCode.UnitTest,       // Уникальный код типа врага   
            "Тестовый юнит",              // Локализованное название                //string nameRu
            200,                          // Максимальное здоровье                  //int maxHP
            5,                            // Базовый урон                           //int dmg
            1,                            // Скорость атаки (атак в секунду)        //float atkSpeed
            1,                            // Скорость передвижения                  //int moveSpeed
            10,                           // Шанс критического удара (%)            //int critChance
            5,                            // Шанс уклонения (%)                     //int evadeChance
            0,                            // Защита (уменьшение получаемого урона)  //int armor
            2,                            // Сопротивление негативным эффектам      //int debuffResist
            0,                            // Вампиризм (% от урона в здоровье)      //int vampire
            2,                            // Скорость полета пуль                   //int bulletFlySpeed
            3,                            // Время жизни пуль (сек)                 //int bulletTimeAlive
            5,                            // Скорость поворота                      //int rotateSpeed
            TypeOfEnemyAttack.TYPE_SHOOT, // Тип атаки (стрельба/ближний бой)       //string typeOfAttack
            1,                            // Количество типов DoT-эффектов          //int countOfDots
            0,                            // Количество модификаторов пуль          //int countOfBulletModifiers
            0,                            // Количество модификаторов стрельбы      //int countOfShootingModifiers
            1,                            // Урон в ближнем бою                     //int meleeDmg
            2,                            // Награда за убийство                    //int deathPrice
            false                         // Является ли боссом                     //bool isBoss
        )}
    };

    // Вспомогательный метод для создания ItemEnemyTypesDictionary с читаемым форматом
    // Позволяет наглядно видеть все параметры при создании нового типа врага
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

    // Получение параметров конкретного типа врага по его коду
    public static ItemEnemyTypesDictionary GetCharacteristic(EnemyTypeCode code)
    {
        // Возвращает параметры врага или null, если тип не найден
        return itemEnemys.TryGetValue(code, out var item) ? item : null;
    }

    // Получение списка всех типов врагов в игре
    public static List<ItemEnemyTypesDictionary> GetAllCharacteristics()
    {
        // Создает новый список на основе значений словаря
        return new List<ItemEnemyTypesDictionary>(itemEnemys.Values);
    }
}