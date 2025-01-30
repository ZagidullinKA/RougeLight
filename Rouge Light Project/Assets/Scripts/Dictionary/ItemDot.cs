
public class ItemDot 
{
    private string code;
    private string nameRu;
    private int upgradeDotDmgX;
    private int upgradeDotDurX;
    private int baseDotDmg;
    private int baseDotDuration;
    private string affectedChar;
    private string type;
    private bool upgradable;

    public string Code => code;
    public string NameRu => nameRu;
    public int UpgradeDotDmgX => upgradeDotDmgX;
    public int UpgradeDotDurX => upgradeDotDurX;
    public int BaseDotDmg => baseDotDmg;
    public int BaseDotDuration => baseDotDuration;
    public string AffectedChar => affectedChar;
    public string Type => type;
    public bool Upgradable => upgradable;

    public ItemDot(string code, string nameRu, int upgradeDotDmgX, 
        int upgradeDotDurX, int baseDotDmg, int baseDotDuration, 
        string affectedChar, string type, bool upgradable)
    {
        this.code = code;
        this.nameRu = nameRu;
        this.upgradeDotDmgX = upgradeDotDmgX;
        this.upgradeDotDurX = upgradeDotDurX;
        this.baseDotDmg = baseDotDmg;
        this.baseDotDuration = baseDotDuration;
        this.affectedChar = affectedChar;
        this.type = type;
        this.upgradable = upgradable;
    }
}
