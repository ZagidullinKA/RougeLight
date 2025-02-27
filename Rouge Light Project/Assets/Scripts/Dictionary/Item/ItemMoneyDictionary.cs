
using UnityEngine;

public class ItemMoneyDictionary 
{
    string code;
    int amount;

    public string Code => code;
    public int Amount => amount;

    public ItemMoneyDictionary(string code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}
