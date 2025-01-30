

public class ItemImprovableCharactes
{
    private string code { get; set; }
    private string nameRu { get; set; }
    private int type { get; set; }
    private int? upgradeAmount { get; set; }
    private int? finalValue { get; set; }
    private int? dmgUpgradeAmount { get; set; }
    private int? durationUpgradeAmount { get; set; }
    private int? finalDotDmg { get; set; }
    private int? finalDotDur { get; set; }

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
