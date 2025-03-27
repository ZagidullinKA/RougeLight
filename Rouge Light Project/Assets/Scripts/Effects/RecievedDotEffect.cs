using System;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class RecievedDotEffect
{
    private string code;
    private string affectedChar;
    private TypeOfDots type;
    private int dotDmg;
    private float dotDur;
    private int affectedDamage;
    private int count;
    private int tick;

    // Конструктор
    public RecievedDotEffect( UsableDotEffect usableDotsArray )
    {
        this.code = usableDotsArray.Code;
        this.affectedChar = usableDotsArray.AffectedChar;
        this.type = usableDotsArray.Type;
        this.dotDmg = usableDotsArray.DotDmg;
        this.dotDur = usableDotsArray.DotDur;
        affectedDamage = 0;
        count = 1;
        tick = 0;
    }

    // Свойства с get/set
    public string Code
    {
        get => code;
        set => code = value;
    }

    public string AffectedChar
    {
        get => affectedChar;
        set => affectedChar = value;
    }

    public TypeOfDots Type
    {
        get => type;
        set => type = value;
    }

    public int DotDmg
    {
        get => dotDmg;
        set => dotDmg = value;
    }

    public float DotDur
    {
        get => dotDur;
        set => dotDur = value;
    }

    public int AffectedDamage
    {
        get => affectedDamage;
        set => affectedDamage = value;
    }

    public int Count
    {
        get => count;
        set => count = value;
    }

    public int Tick
    {
        get => tick;
        set => tick = value;
    }

    
    
}