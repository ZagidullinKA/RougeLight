
using System;
using System.Collections.Generic;

public static class ImprovableCharactesDictionary
{
    private static readonly List<ItemImprovableCharactes> ItemsImprovableCharactes = new();


    public static List<ItemImprovableCharactes> getListItemImprovableCharactes()
    {
        if (ItemsImprovableCharactes.Count != 0)
            ItemsImprovableCharactes.Clear();

        foreach (var itemCharacter in DictionaryCharacters.GetAllCharacteristics())
        {
            if (itemCharacter.Upgradable == true) {
                ItemImprovableCharactes item = new(
                    itemCharacter.Code,
                    itemCharacter.NameRu,
                    1,
                    0,
                    itemCharacter.BaseAmount,
                    null,
                    null,
                    null,
                    null);

                ItemsImprovableCharactes.Add(item);
            }
        }

        foreach (var itemDot in DotsDictionary.GetAllDots())
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

    public static ItemImprovableCharactes GetImprovableCharacteristic(string code)
    {
        return ItemsImprovableCharactes.Find(x => x.Code == code);
    }

    public static List<ItemImprovableCharactes> GetAllImprovableCharacteristics()
    {
        getListItemImprovableCharactes();
        return ItemsImprovableCharactes;
    }
}
