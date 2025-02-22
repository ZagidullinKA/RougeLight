
public class ItemCharacter
{
    private string code;
    private string nameRu;
    private bool upgradable;
    private int upgradeX;
    private int baseAmount;
    private int price;
    private bool isEnemyAvaliable;

    public string Code => code;
    public string NameRu => nameRu;
    public bool Upgradable => upgradable;
    public int UpgradeX => upgradeX;
    public int BaseAmount => baseAmount;
    public int Price => price;
    public bool IsEnemyAvaliable => isEnemyAvaliable;

    public ItemCharacter(string code, string nameRu, bool upgradable, int upgradeX, int baseAmount, int price, bool isEnemyAvaliable)
    {
        this.code = code;
        this.nameRu = nameRu;
        this.upgradable = upgradable;
        this.upgradeX = upgradeX;
        this.baseAmount = baseAmount;
        this.price = price;
        this.isEnemyAvaliable = isEnemyAvaliable;
    }


}
