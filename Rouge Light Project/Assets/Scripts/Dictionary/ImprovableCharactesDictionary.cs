
using log4net;
using System;
using System.Collections.Generic;

public static class ImprovableCharactesDictionary
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Hero));

    private static readonly List<ItemImprovableCharactes> ItemsImprovableCharactes = new();


    public static List<ItemImprovableCharactes> getListItemImprovableCharactes()
    {
        if (ItemsImprovableCharactes.Count != 0)
            ItemsImprovableCharactes.Clear();

        foreach (var itemCharacter in DictionaryCharacters.GetAllCharacteristics())
        {
            if (itemCharacter.Upgradable == true) {
                
                ValidationValue.ValidateStringNotNullOrEmpty(
                    (itemCharacter.Code, nameof(itemCharacter.Code)),
                    (itemCharacter.NameRu, nameof(itemCharacter.NameRu))
                    );

                ValidationValue.ValidateIntNotNull(
                     (itemCharacter.BaseAmount, nameof(itemCharacter.BaseAmount))
                     );

                ItemImprovableCharactes item = new(
                    itemCharacter.Code,
                    itemCharacter.NameRu,
                    true,
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
                ValidationValue.ValidateStringNotNullOrEmpty(
                    (itemDot.Code, nameof(itemDot.Code)),
                    (itemDot.NameRu, nameof(itemDot.NameRu))
                    );

                ValidationValue.ValidateIntNotNull(
                     (itemDot.BaseDotDmg, nameof(itemDot.BaseDotDmg)),
                     (itemDot.BaseDotDuration, nameof(itemDot.BaseDotDuration))
                     );

                ItemImprovableCharactes item = new(
                    itemDot.Code,
                    itemDot.NameRu,
                    false,
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
