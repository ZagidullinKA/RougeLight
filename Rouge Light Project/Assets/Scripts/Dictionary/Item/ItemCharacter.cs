
public class ItemCharacter
{
    private CharacterStatCode code;
    private string nameRu;
    private bool upgradable;
    private int upgradeX;
    private float baseAmount;
    private int price;
    private bool isEnemyAvailable;

    public CharacterStatCode Code => code;
    public string NameRu => nameRu;
    public bool Upgradable => upgradable;
    public int UpgradeX => upgradeX;
    public float BaseAmount => baseAmount;
    public int Price => price;
    public bool IsEnemyAvailable => isEnemyAvailable;

    public ItemCharacter(CharacterStatCode code, string nameRu, bool upgradable, int upgradeX, float baseAmount, int price, bool isEnemyAvailable)
    {
        this.code = code;
        this.nameRu = nameRu;
        this.upgradable = upgradable;
        this.upgradeX = upgradeX;
        this.baseAmount = baseAmount;
        this.price = price;
        this.isEnemyAvailable = isEnemyAvailable;
    }
}