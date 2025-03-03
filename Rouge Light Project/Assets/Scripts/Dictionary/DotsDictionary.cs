
using System.Collections.Generic;

public static class DotsDictionary 
{
    private static readonly List<ItemDot> ITEM_DOTS = new()
        {
            new ItemDot("Fire", "ќгонь", 1, 1, 1, 1, "moveSpeed", TypeOfDots.TYPE_PERCENT, true),
            new ItemDot("Poison", "яд", 2, 2, 2, 2, "moveSpeed", TypeOfDots.TYPE_FIXED, true)
        };

    public static ItemDot GetDot(string code)
    {
        return ITEM_DOTS.Find(x => x.Code == code);
    }

    public static List<ItemDot> GetAllDots()
    {
        return ITEM_DOTS;
    }
}
