// Класс ItemDot представляет параметры Damage-over-Time (DoT) эффекта
// Содержит все необходимые данные для создания и управления периодическим уроном
public class ItemDot
{
    // Приватные поля класса:
    private DotCode code;                  // Тип эффекта (огонь, яд и т.д.)
    private string nameRu;                 // Локализованное название на русском
    private int upgradeDotDmgX;            // Шаг увеличения урона при улучшении
    private int upgradeDotDurX;            // Шаг увеличения длительности при улучшении
    private int baseDotDmg;                // Базовый урон эффекта
    private int baseDotDuration;           // Базовая длительность эффекта (в тиках)
    private CharacterStatCode affectedChar; // Характеристика, на которую влияет эффект
    private TypeOfDots type;               // Тип расчета урона (фиксированный/процентный)
    private bool upgradable;               // Можно ли улучшать этот эффект

    // Свойства только для чтения:
    public DotCode Code => code;           // Возвращает тип эффекта
    public string NameRu => nameRu;        // Возвращает локализованное название
    public int UpgradeDotDmgX => upgradeDotDmgX; // Возвращает шаг улучшения урона
    public int UpgradeDotDurX => upgradeDotDurX; // Возвращает шаг улучшения длительности
    public int BaseDotDmg => baseDotDmg;   // Возвращает базовый урон
    public int BaseDotDuration => baseDotDuration; // Возвращает базовую длительность
    public CharacterStatCode AffectedChar => affectedChar; // Возвращает целевую характеристику
    public TypeOfDots Type => type;        // Возвращает тип расчета урона
    public bool Upgradable => upgradable;  // Возвращает возможность улучшения

    // Конструктор класса:
    public ItemDot(DotCode code, string nameRu, int upgradeDotDmgX, int upgradeDotDurX,
                   int baseDotDmg, int baseDotDuration, CharacterStatCode affectedChar,
                   TypeOfDots type, bool upgradable)
    {
        // Инициализация полей:
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