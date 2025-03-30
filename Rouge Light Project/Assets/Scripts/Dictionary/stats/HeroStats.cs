using UnityEngine;
using System.Collections.Generic;
using System;
using log4net;

[CreateAssetMenu(fileName = "HeroStats", menuName = "Character/HeroStats")]
public class HeroStats : BaseStats
{
    private static readonly ILog log = LogManager.GetLogger(typeof(HeroStats));

    [SerializeField] protected int luck = 0;                     // Удача, специфичная для героев
    [SerializeField] protected bool isGodMode = true;           //Переменная для godmod

    // Свойство с прямым get и set для luck
    public int Luck { get => luck; set => luck = value; }
    public bool IsGodMode { get => isGodMode; set => isGodMode = value; }

    // Переопределение инициализации для учёта luck
    public override void InitializeFromDictionary(DictionaryCharacters dictionary)
    {
        base.InitializeFromDictionary(dictionary); // Вызываем базовую инициализацию
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            if (item.Code == CharacterStatCode.Luck)
            {
                Luck = (int)item.BaseAmount; // Устанавливаем luck напрямую
            }
        }
    }
}