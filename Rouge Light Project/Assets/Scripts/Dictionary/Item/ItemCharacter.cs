
public class ItemCharacter
{
    private string code;
    private string nameRu;
    private bool upgradable;
    private int upgradeX;
    private int baseAmount;
    private int price;

    public string Code => code;
    public string NameRu => nameRu;
    public bool Upgradable => upgradable;
    public int UpgradeX => upgradeX;
    public int BaseAmount => baseAmount;
    public int Price => price;

    public ItemCharacter(string code, string nameRu, bool upgradable, int upgradeX, int baseAmount, int price)
    {
        this.code = code;
        this.nameRu = nameRu;
        this.upgradable = upgradable;
        this.upgradeX = upgradeX;
        this.baseAmount = baseAmount;
        this.price = price;
    }


}
