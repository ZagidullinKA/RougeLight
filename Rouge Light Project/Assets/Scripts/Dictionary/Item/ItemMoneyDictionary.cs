

public class ItemMoneyDictionary
{
    private MoneyCode code;
    private int amount;

    public MoneyCode Code => code;
    public int Amount
    {
        get => amount;
        set => amount = value; // Оставляем setter для изменения Amount
    }

    public ItemMoneyDictionary(MoneyCode code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}