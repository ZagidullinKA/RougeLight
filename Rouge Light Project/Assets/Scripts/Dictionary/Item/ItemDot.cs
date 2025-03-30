
public class ItemDot
{
    private DotCode code;
    private string nameRu;
    private int upgradeDotDmgX;
    private int upgradeDotDurX;
    private int baseDotDmg;
    private int baseDotDuration;
    private CharacterStatCode affectedChar;
    private TypeOfDots type;
    private bool upgradable;

    public DotCode Code => code;
    public string NameRu => nameRu;
    public int UpgradeDotDmgX => upgradeDotDmgX;
    public int UpgradeDotDurX => upgradeDotDurX;
    public int BaseDotDmg => baseDotDmg;
    public int BaseDotDuration => baseDotDuration;
    public CharacterStatCode AffectedChar => affectedChar;
    public TypeOfDots Type => type;
    public bool Upgradable => upgradable;

    public ItemDot(DotCode code, string nameRu, int upgradeDotDmgX, int upgradeDotDurX,
                   int baseDotDmg, int baseDotDuration, CharacterStatCode affectedChar,
                   TypeOfDots type, bool upgradable)
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
