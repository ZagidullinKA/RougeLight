public class ItemEnemyTypesDictionary
{
    // Приватные поля
    private string code;
    private string nameRu;
    private int maxHP;
    private int dmg;
    private float atkSpeed;
    private int moveSpeed;
    private int critChance;
    private int evadeChance;
    private int armor;
    private int debuffResist;
    private int vampire;
    private int bulletFlySpeed;
    private int bulletTimeAlive;
    private int rotateSpeed;
    private string typeOfAttack;
    private int countOfDots;
    private int countOfBulletModifiers;
    private int countOfShootingModifiers;
    private int meleeDmg;
    private int deathPrice;
    private bool isBoss;

    // Свойства только для чтения (get'ры)
    public string Code => code;
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
    public string TypeOfAttack => typeOfAttack;
    public int CountOfDots => countOfDots;
    public int CountOfBulletModifiers => countOfBulletModifiers;
    public int CountOfShootingModifiers => countOfShootingModifiers;
    public int MeleeDmg => meleeDmg;
    public int DeathPrice => deathPrice;
    public bool IsBoss => isBoss;

    // Конструктор
    public ItemEnemyTypesDictionary ( 
        string code,
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
        string typeOfAttack,
        int countOfDots,
        int countOfBulletModifiers,
        int countOfShootingModifiers,
        int meleeDmg,
        int deathPrice,
        bool isBoss)
    {
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

        ValidationValue.ValidateFloatNotNull(
            (atkSpeed, nameof(atkSpeed))
            );

        ValidationValue.ValidateStringNotNullOrEmpty(
            (code, nameof(code)),
            (nameRu, nameof(nameRu)),
            (typeOfAttack, nameof(typeOfAttack))
            );



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