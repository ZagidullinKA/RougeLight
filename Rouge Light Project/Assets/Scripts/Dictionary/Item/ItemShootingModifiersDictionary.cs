public class ItemShootingModifiersDictionary
{
    private TypeOfShootingModifier code;    // Код модификатора
    private int lvl;        // Уровень модификатора
    private int count;      // Количество выстрелов модификатора
    private string type;    // Тип модификатора
    private float accuracy; // Точность модификатора
    private int[] range;    // Диапазон углов для модификатора [start, end] (может быть null)

    // Свойство для доступа к полю code
    public TypeOfShootingModifier Code
    {
        get => code;
        set => code = value;
    }

    // Свойство для доступа к полю lvl
    public int Lvl
    {
        get => lvl;
        set => lvl = value;
    }

    // Свойство для доступа к полю count
    public int Count
    {
        get => count;
        set => count = value;
    }

    // Свойство для доступа к полю type
    public string Type
    {
        get => type;
        set => type = value;
    }

    // Свойство для доступа к полю accuracy
    public float Accuracy
    {
        get => accuracy;
        set => accuracy = value;
    }

    // Свойство для доступа к полю range
    public int[] Range
    {
        get => range;
        set => range = value;
    }

    // Конструктор для создания нового объекта класса
    public ItemShootingModifiersDictionary(TypeOfShootingModifier code, int lvl, int count, string type, float accuracy, int[] range)
    {
        this.code = code;
        this.lvl = lvl;
        this.count = count;
        this.type = type;
        this.accuracy = accuracy;
        this.range = range;
    }
}