using System;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class UsableDotEffect
{
    private string code;             // Уникальный код эффекта
    private int dotDmg;             // Финальный урон за тик
    private float dotDur;           // Финальная длительность эффекта
    private int dmgUpgCount;        // Количество улучшений урона
    private int durUpgCount;        // Количество улучшений длительности
    private string affectedChar;    // Идентификатор затронутого персонажа
    private TypeOfDots type;            // Тип эффекта (например, "Poison", "Fire")

    public UsableDotEffect(string code, int finalDotDmg, float finalDotDur, int dmgUpgCount, int durUpgCount)
    {
        this.code = code;
        this.DotDmg = finalDotDmg;
        this.DotDur = finalDotDur;
        this.DmgUpgCount = dmgUpgCount;
        this.DurUpgCount = durUpgCount;

        var dotItem = DotsDictionary.GetDot(code);
        ValidationValue.ValidateStringNotNullOrEmpty(
                        (dotItem.AffectedChar, "upgradableItem.AffectedChar")
                    );

        this.affectedChar = dotItem.AffectedChar;
        this.type = dotItem.Type;
    }

    public string Code
    {
        get { return code; }
        set { code = value; }
    }

    public int DotDmg
    {
        get { return dotDmg; }
        set { dotDmg = value; }
    }

    public float DotDur
    {
        get { return dotDur; }
        set { dotDur = value; }
    }

    public int DmgUpgCount
    {
        get { return dmgUpgCount; }
        set { dmgUpgCount = value; }
    }

    public int DurUpgCount
    {
        get { return durUpgCount; }
        set { durUpgCount = value; }
    }


    public string AffectedChar
    {
        get { return affectedChar; }
        set { affectedChar = value; }
    }


    public TypeOfDots Type
    {
        get { return type; }
        set { type = value; }
    }
}