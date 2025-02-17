
using System.Collections.Generic;

public static class DotsDictionary 
{
    private static readonly List<ItemDot> ITEM_DOTS = new()
        {
            new ItemDot("Fire", "Огонь", 1, 1, 1, 1, "actualHP", TypeOfDots.TYPE_PERCENT, true)
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
