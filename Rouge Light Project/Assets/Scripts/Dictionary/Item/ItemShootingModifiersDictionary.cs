
public class ItemShootingModifiersDictionary
{
    private TypeOfShootingModifier code;    // Код модификатора
    private int lvl;        // Уровень модификатора
    private int count;      // Количество применений модификатора
    private string type;    // Тип модификатора
    private float accuracy; // Точность модификатора

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

    // Конструктор для инициализации всех полей
    public ItemShootingModifiersDictionary(TypeOfShootingModifier code, int lvl, int count, string type, float accuracy)
    {
        this.code = code;
        this.lvl = lvl;
        this.count = count;
        this.type = type;
        this.accuracy = accuracy;
    }
}
