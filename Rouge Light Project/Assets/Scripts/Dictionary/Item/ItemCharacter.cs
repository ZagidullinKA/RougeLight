// Класс ItemCharacter хранит данные об игровых характеристиках персонажей
// Используется для настройки и управления параметрами персонажей и врагов
public class ItemCharacter
{
    // Приватные поля класса:
    private CharacterStatCode code;        // Идентификатор характеристики из перечисления
    private string nameRu;                 // Локализованное название на русском языке
    private bool upgradable;               // Можно ли улучшать эту характеристику
    private int upgradeX;                  // Величина улучшения за один уровень
    private float baseAmount;              // Базовое значение характеристики
    private int price;                     // Стоимость улучшения (в игровой валюте)
    private bool isEnemyAvailable;         // Доступна ли характеристика для врагов

    // Свойства только для чтения:
    public CharacterStatCode Code => code; // Возвращает идентификатор характеристики
    public string NameRu => nameRu;        // Возвращает локализованное название
    public bool Upgradable => upgradable;  // Возвращает возможность улучшения
    public int UpgradeX => upgradeX;       // Возвращает шаг улучшения
    public float BaseAmount => baseAmount; // Возвращает базовое значение
    public int Price => price;             // Возвращает стоимость улучшения
    public bool IsEnemyAvailable => isEnemyAvailable; // Возвращает доступность для врагов

    // Конструктор класса:
    public ItemCharacter(CharacterStatCode code, string nameRu, bool upgradable,
                        int upgradeX, float baseAmount, int price, bool isEnemyAvailable)
    {
        // Инициализация всех полей:
        this.code = code;
        this.nameRu = nameRu;
        this.upgradable = upgradable;
        this.upgradeX = upgradeX;
        this.baseAmount = baseAmount;
        this.price = price;
        this.isEnemyAvailable = isEnemyAvailable;
    }
}