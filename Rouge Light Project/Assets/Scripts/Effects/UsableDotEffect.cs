using System;
using UnityEngine;
using static UnityEditor.Progress;

// Класс UsableDotEffect представляет активный DoT-эффект, который может использовать персонаж
// Содержит текущие параметры эффекта и методы для работы с ними
[System.Serializable]
public class UsableDotEffect
{
    // Приватные поля класса:
    private DotCode code;                       // Уникальный код эффекта (из перечисления DotCode)
    private int dotDmg;                         // Финальный урон за тик (с учетом всех модификаторов)
    private float dotDur;                       // Финальная длительность эффекта (в секундах/тиках)
    private CharacterStatCode affectedChar;     // Идентификатор затронутой характеристики (например, здоровье)
    private TypeOfDots type;                    // Тип эффекта (например, "Poison", "Fire")

    // Конструктор класса:
    public UsableDotEffect(DotCode code, int finalDotDmg, float finalDotDur)
    {
        // Инициализация основных параметров эффекта
        this.code = code;
        this.DotDmg = finalDotDmg;
        this.DotDur = finalDotDur;

        // Получение базовых параметров эффекта из словаря
        var dotItem = DotsDictionary.GetDot(code);

        // Валидация параметров эффекта
        ValidationValue.ValidateStringNotNullOrEmpty(
            (dotItem.AffectedChar.ToString(), "upgradableItem.AffectedChar")
        );

        // Установка дополнительных параметров из словаря
        this.affectedChar = dotItem.AffectedChar;
        this.type = dotItem.Type;
    }

    // Свойства с get/set для доступа к параметрам:

    public DotCode Code
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

    public CharacterStatCode AffectedChar
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