

public class ItemImprovableCharactes
{
    private string code;
    private string nameRu;
    private int type;
    private int? upgradeAmount;
    private int? finalValue;
    private int? dmgUpgradeAmount;
    private int? durationUpgradeAmount;
    private int? finalDotDmg;
    private int? finalDotDur;   

    public string Code => code;
    public string Name => nameRu;       
    public int Type => type;
    public int? UpgradeAmount => upgradeAmount;
    public int? FinalValue => finalValue;
    public int? DmgUpgradeAmount => dmgUpgradeAmount;  
    public int? DurationUpgradeAmount => durationUpgradeAmount;
    public int? FinalDotDmg => finalDotDmg;
    public int? FinalDotDur => finalDotDur;

    public ItemImprovableCharactes(string code, string nameRu, int type, 
        int? upgradeAmount, int? finalValue, int? dmgUpgradeAmount, 
        int? durationUpgradeAmount, int? finalDotDmg, int? finalDotDur)
    {
        this.code = code;
        this.nameRu = nameRu;
        this.type = type;
        this.upgradeAmount = upgradeAmount;
        this.finalValue = finalValue;
        this.dmgUpgradeAmount = dmgUpgradeAmount;
        this.durationUpgradeAmount = durationUpgradeAmount;
        this.finalDotDmg = finalDotDmg;
        this.finalDotDur = finalDotDur;
    }
}
