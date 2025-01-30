
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class ImprovableCharactesDictionary
{
    public static readonly List<ItemImprovableCharactes> ItemsImprovableCharactes = new();


    public static List<ItemImprovableCharactes> getListItemImprovableCharactes()
    {
        if (ItemsImprovableCharactes.Count != 0)
            ItemsImprovableCharactes.Clear();

        foreach (var itemCharacter in DictionaryCharacters.itemCharacters)
        {
            if (itemCharacter.Upgradable == true) {
                ItemImprovableCharactes item = new(
                    itemCharacter.Code,
                    itemCharacter.NameRu,
                    1,
                    0,
                    itemCharacter.BaseAmount * itemCharacter.UpgradeX,
                    null,
                    null,
                    null,
                    null);

                ItemsImprovableCharactes.Add(item);
            }
        }

        foreach (var itemDot in DotsDictionary.ITEM_DOTS)
        {
            if (itemDot.Upgradable == true)
            {
                ItemImprovableCharactes item = new(
                    itemDot.Code,
                    itemDot.NameRu,
                    0,
                    null,
                    null,
                    0,
                    0,
                    itemDot.BaseDotDmg,
                    itemDot.BaseDotDuration);

                ItemsImprovableCharactes.Add(item);
            }
        }

        return ItemsImprovableCharactes;
    }
}
