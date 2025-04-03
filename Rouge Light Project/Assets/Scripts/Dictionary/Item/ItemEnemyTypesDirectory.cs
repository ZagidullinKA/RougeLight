// Класс ItemEnemyTypesDictionary представляет полный набор характеристик для конкретного типа врага
// Содержит все параметры, необходимые для создания и настройки вражеских юнитов
public class ItemEnemyTypesDictionary
{
    // Основные поля класса (все приватные, доступ через свойства)
    private EnemyTypeCode code;            // Уникальный идентификатор типа врага
    private string nameRu;                 // Локализованное название на русском
    private int maxHP;                     // Максимальное здоровье
    private int dmg;                       // Базовый урон
    private float atkSpeed;                // Скорость атаки (атак в секунду)
    private int moveSpeed;                 // Скорость перемещения
    private int critChance;                // Шанс критического удара (%)
    private int evadeChance;               // Шанс уклонения (%)
    private int armor;                     // Защита (снижение урона)
    private int debuffResist;              // Сопротивление негативным эффектам
    private int vampire;                   // Вампиризм (% урона в здоровье)
    private int bulletFlySpeed;            // Скорость снарядов
    private int bulletTimeAlive;           // Время жизни снарядов
    private int rotateSpeed;               // Скорость поворота
    private TypeOfEnemyAttack typeOfAttack; // Тип атаки (дальняя/ближняя)
    private int countOfDots;               // Количество доступных DoT-эффектов
    private int countOfBulletModifiers;    // Количество модификаторов снарядов
    private int countOfShootingModifiers;  // Количество модификаторов стрельбы
    private int meleeDmg;                  // Урон в ближнем бою
    private int deathPrice;                // Награда за убийство
    private bool isBoss;                   // Является ли боссом

    // Свойства только для чтения (инкапсуляция полей)
    public EnemyTypeCode Code => code;
    public string NameRu => nameRu;
    public int MaxHP => maxHP;
    public int Dmg => dmg;
    public float AtkSpeed => atkSpeed;
    public int MoveSpeed => moveSpeed;
    public int CritChance => critChance;
    public int EvadeChance => evadeChance;
    public int Armor => armor;
    public int DebuffResist => debuffResist;
    public int Vampire => vampire;
    public int BulletFlySpeed => bulletFlySpeed;
    public int BulletTimeAlive => bulletTimeAlive;
    public int RotateSpeed => rotateSpeed;
    public TypeOfEnemyAttack TypeOfAttack => typeOfAttack;
    public int CountOfDots => countOfDots;
    public int CountOfBulletModifiers => countOfBulletModifiers;
    public int CountOfShootingModifiers => countOfShootingModifiers;
    public int MeleeDmg => meleeDmg;
    public int DeathPrice => deathPrice;
    public bool IsBoss => isBoss;

    // Конструктор с валидацией параметров
    public ItemEnemyTypesDictionary(
        EnemyTypeCode code,
        string nameRu,
        int maxHP,
        int dmg,
        float atkSpeed,
        int moveSpeed,
        int critChance,
        int evadeChance,
        int armor,
        int debuffResist,
        int vampire,
        int bulletFlySpeed,
        int bulletTimeAlive,
        int rotateSpeed,
        TypeOfEnemyAttack typeOfAttack,
        int countOfDots,
        int countOfBulletModifiers,
        int countOfShootingModifiers,
        int meleeDmg,
        int deathPrice,
        bool isBoss)
    {
        // Валидация целочисленных параметров
        ValidationValue.ValidateIntNotNull(
            (maxHP, nameof(maxHP)),
            (dmg, nameof(dmg)),
            (moveSpeed, nameof(moveSpeed)),
            (critChance, nameof(critChance)),
            (evadeChance, nameof(evadeChance)),
            (armor, nameof(armor)),
            (debuffResist, nameof(debuffResist)),
            (vampire, nameof(vampire)),
            (rotateSpeed, nameof(rotateSpeed)),
            (countOfDots, nameof(countOfDots)),
            (countOfBulletModifiers, nameof(countOfBulletModifiers)),
            (countOfShootingModifiers, nameof(countOfShootingModifiers)),
            (meleeDmg, nameof(meleeDmg)),
            (deathPrice, nameof(deathPrice)),
            (bulletFlySpeed, nameof(bulletFlySpeed)),
            (bulletTimeAlive, nameof(bulletTimeAlive))
        );

        // Валидация параметров с плавающей точкой
        ValidationValue.ValidateFloatNotNull(
            (atkSpeed, nameof(atkSpeed))
        );

        // Валидация строковых параметров
        ValidationValue.ValidateStringNotNullOrEmpty(
            (nameRu, nameof(nameRu))
        );

        // Инициализация полей
        this.code = code;
        this.nameRu = nameRu;
        this.maxHP = maxHP;
        this.dmg = dmg;
        this.atkSpeed = atkSpeed;
        this.moveSpeed = moveSpeed;
        this.critChance = critChance;
        this.evadeChance = evadeChance;
        this.armor = armor;
        this.debuffResist = debuffResist;
        this.vampire = vampire;
        this.bulletFlySpeed = bulletFlySpeed;
        this.bulletTimeAlive = bulletTimeAlive;
        this.rotateSpeed = rotateSpeed;
        this.typeOfAttack = typeOfAttack;
        this.countOfDots = countOfDots;
        this.countOfBulletModifiers = countOfBulletModifiers;
        this.countOfShootingModifiers = countOfShootingModifiers;
        this.meleeDmg = meleeDmg;
        this.deathPrice = deathPrice;
        this.isBoss = isBoss;
    }
}