//  ласс ItemMoneyDictionary представл€ет данные о денежной единице в игре
// —одержит информацию о типе валюты и еЄ количестве
public class ItemMoneyDictionary
{
    // ѕоле дл€ хранени€ типа денежной единицы (из перечислени€ MoneyCode)
    private MoneyCode code;

    // ѕоле дл€ хранени€ текущего количества денег
    private int amount;

    // —войство только дл€ чтени€, возвращающее тип денежной единицы
    public MoneyCode Code => code;

    // —войство дл€ доступа к количеству денег с возможностью изменени€
    public int Amount
    {
        get => amount;
        set => amount = value; // ќставл€ем setter дл€ изменени€ Amount
    }

    //  онструктор класса, инициализирующий денежную единицу
    // ѕараметры:
    // code - тип денежной единицы (из MoneyCode)
    // amount - начальное количество
    public ItemMoneyDictionary(MoneyCode code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}