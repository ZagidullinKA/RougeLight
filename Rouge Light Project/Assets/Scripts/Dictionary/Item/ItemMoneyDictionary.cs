using System;

public class ItemMoneyDictionary
{
    private string code;
    private int amount;

    public string Code
    {
        get => code;
        set { code = value; }
    }

    public int Amount
    {
        get => amount;
        set { amount = value; }
    }

    public ItemMoneyDictionary(string code, int amount)
    {
        Code = code; 
        Amount = amount;
    }
}