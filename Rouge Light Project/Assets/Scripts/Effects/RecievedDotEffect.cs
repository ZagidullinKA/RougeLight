using System;
using UnityEngine;
using static UnityEditor.Progress;

// Класс RecievedDotEffect представляет активный DoT-эффект, примененный к персонажу
// Отслеживает текущее состояние эффекта и его параметры во время действия
[System.Serializable]
public class RecievedDotEffect
{
    // Приватные поля класса:
    private DotCode code;                  // Тип эффекта (из перечисления DotCode)
    private CharacterStatCode affectedChar; // Характеристика, на которую влияет эффект
    private TypeOfDots type;               // Способ расчета урона (фиксированный/процентный)
    private int dotDmg;                    // Текущий урон за тик
    private float dotDur;                  // Оставшееся время действия эффекта
    private int affectedDamage;            // Накопленный урон/изменение характеристики
    private int count;                     // Количество стаков эффекта
    private int tick;                      // Счетчик тиков (для периодических эффектов)

    // Конструктор класса:
    public RecievedDotEffect(UsableDotEffect usableDotsArray)
    {
        // Инициализация параметров из базового эффекта:
        this.code = usableDotsArray.Code;
        this.affectedChar = usableDotsArray.AffectedChar;
        this.type = usableDotsArray.Type;
        this.dotDmg = usableDotsArray.DotDmg;
        this.dotDur = usableDotsArray.DotDur;

        // Инициализация счетчиков:
        affectedDamage = 0;    // Накопленный урон/изменение характеристики
        count = 1;             // Начальное количество стаков
        tick = 0;              // Счетчик тиков
    }

    // Свойства с get/set для доступа к параметрам:

    public DotCode Code
    {
        get => code;
        set => code = value;
    }

    public CharacterStatCode AffectedChar
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