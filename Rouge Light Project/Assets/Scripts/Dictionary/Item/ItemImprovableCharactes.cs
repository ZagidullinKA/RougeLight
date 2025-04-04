// Класс ItemImprovableCharactes хранит данные об улучшаемых характеристиках и эффектах
// Объединяет параметры для обычных характеристик и DoT-эффектов в одной структуре
public class ItemImprovableCharactes
{
    // Поля класса:
    private string code; // Оставляем string, так как коды из разных enum будут конвертироваться в строки
    private string nameRu; // Локализованное название на русском
    private bool type; // true = характеристика (CharacterStatCode), false = DoT-эффект (DotCode)
    private int? upgradeAmount; // Величина улучшения для характеристик
    private float? finalValue; // Финальное значение характеристики после улучшения
    private int? dmgUpgradeAmount; // Увеличение урона для DoT-эффектов
    private int? durationUpgradeAmount; // Увеличение длительности для DoT-эффектов
    private int? finalDotDmg; // Финальный урон DoT-эффекта
    private int? finalDotDur; // Финальная длительность DoT-эффекта

    // Свойства только для чтения:
    public string Code => code; // Возвращает код характеристики/эффекта
    public string Name => nameRu; // Возвращает локализованное название
    public bool Type => type; // Возвращает тип (true - характеристика, false - DoT)
    public int? UpgradeAmount => upgradeAmount; // Возвращает величину улучшения
    public float? FinalValue => finalValue; // Возвращает финальное значение характеристики
    public int? DmgUpgradeAmount => dmgUpgradeAmount; // Возвращает улучшение урона DoT
    public int? DurationUpgradeAmount => durationUpgradeAmount; // Возвращает улучшение длительности DoT
    public int? FinalDotDmg => finalDotDmg; // Возвращает финальный урон DoT
    public int? FinalDotDur => finalDotDur; // Возвращает финальную длительность DoT

    // Конструктор класса:
    public ItemImprovableCharactes(string code, string nameRu, bool type,
        int? upgradeAmount, float? finalValue, int? dmgUpgradeAmount,
        int? durationUpgradeAmount, int? finalDotDmg, int? finalDotDur)
    {
        // Инициализация полей:
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