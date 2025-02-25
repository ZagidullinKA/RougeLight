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
    private string typeOfAttack;

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
    public int BulletflySpeed => bulletFlySpeed;
    public int BulletTimeAlive => bulletTimeAlive;
    public string TypeOfAttack => typeOfAttack;

    // Конструктор
    public ItemEnemyTypesDictionary (
        string code, string nameRu, int maxHP, int dmg, float atkSpeed, int moveSpeed,
        int critChance, int evadeChance, int armor, int debuffResist, int vampire,
        int bulletFlySpeed, int bulletTimeAlive, string typeOfAttack)
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
        this.typeOfAttack = typeOfAttack;
    }
}