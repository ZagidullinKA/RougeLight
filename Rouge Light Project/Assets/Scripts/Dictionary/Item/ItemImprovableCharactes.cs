

public class ItemImprovableCharactes
{
    private string code; // Оставляем string, так как коды из разных enum будут конвертироваться в строки
    private string nameRu;
    private bool type; // true = характеристика (CharacterStatCode), false = DoT-эффект (DotCode)
    private int? upgradeAmount;
    private float? finalValue;
    private int? dmgUpgradeAmount;
    private int? durationUpgradeAmount;
    private int? finalDotDmg;
    private int? finalDotDur;

    public string Code => code;
    public string Name => nameRu;
    public bool Type => type;
    public int? UpgradeAmount => upgradeAmount;
    public float? FinalValue => finalValue;
    public int? DmgUpgradeAmount => dmgUpgradeAmount;
    public int? DurationUpgradeAmount => durationUpgradeAmount;
    public int? FinalDotDmg => finalDotDmg;
    public int? FinalDotDur => finalDotDur;

    public ItemImprovableCharactes(string code, string nameRu, bool type,
        int? upgradeAmount, float? finalValue, int? dmgUpgradeAmount,
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