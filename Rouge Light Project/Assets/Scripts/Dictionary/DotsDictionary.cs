
using System.Collections.Generic;

public static class DotsDictionary 
{
    public static readonly List<ItemDot> ITEM_DOTS = new()
        {
            new ItemDot("Fire", "Огонь", 1, 1, 1, 1, "actualHP", TypeOfDots.TYPE_PERCENT, true)
        };
}
