using log4net;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class ImprovableCharactesDictionary
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Hero));

    private static readonly List<ItemImprovableCharactes> ItemsImprovableCharactes = new();


    public static List<ItemImprovableCharactes> getListItemImprovableCharactesAndDots()
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

                ValidationValue.ValidateFloatNotNull(
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
            UnityEngine.Debug.Log("Type = " + itemDot.Type + " Code = " + itemDot.Code + " Upgradable = " + itemDot.Upgradable);
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
                UnityEngine.Debug.Log("Code = " + item.Code + " Добавлен");
            }
        }

        return ItemsImprovableCharactes;
    }

    public static ItemImprovableCharactes GetImprovableCharacteristicOrDot(string code)
    {
        return ItemsImprovableCharactes.Find(x => x.Code == code);
    }

    public static List<ItemImprovableCharactes> GetAllImprovableCharacteristicsAndDots()
    {
        getListItemImprovableCharactesAndDots();
        return ItemsImprovableCharactes;
    }

    public static List<ItemImprovableCharactes> GetAllImprovableDots()
    {
        List < ItemImprovableCharactes > listDots = new();
        foreach (var item in getListItemImprovableCharactesAndDots())
        {
            if (item.Type == false)
            {
                listDots.Add(item);
            }
        }

        return listDots;
    }

    public static int GetFinalDotDmgOfCode(string code)
    {
        int? dotDmgReturn = ItemsImprovableCharactes.Find(x => x.Code == code).FinalDotDmg;

        if (dotDmgReturn == null)
        {
            log.Error("Не верный code или поле dotDmg не заполнено, code = " + code);
        }

        return (int) dotDmgReturn;
    }

    public static int GetFinalDotDurOfCode(string code)
    {
        int? dotDurReturn = ItemsImprovableCharactes.Find(x => x.Code == code).FinalDotDur;

        if (dotDurReturn == null)
        {
            log.Error("Не верный code или поле dotDmg не заполнено, code = " + code);
        }

        return (int) dotDurReturn;
    }
}
