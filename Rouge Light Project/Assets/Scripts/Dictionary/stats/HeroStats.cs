using UnityEngine;
using System.Collections.Generic;
using System;
using log4net;

// Класс HeroStats наследуется от BaseStats и содержит специфичные для игрового героя параметры
// Представляет ScriptableObject для настройки характеристик героя через редактор Unity
[CreateAssetMenu(fileName = "HeroStats", menuName = "Character/HeroStats")]
public class HeroStats : BaseStats
{
    // Инициализация логгера для записи событий и ошибок
    private static readonly ILog log = LogManager.GetLogger(typeof(HeroStats));

    // Уникальные характеристики героя:
    [SerializeField] protected int luck = 0;                    // Удача, влияет на случайные события (криты, дроп и др.)
    [SerializeField] protected bool isGodMode = true;           // Режим бога (неуязвимость) для тестирования
    [SerializeField] protected int dropRadius = 0;              // Радиус автоматического подбора предметов

    // Свойства с прямым доступом для управления параметрами:
    public int Luck { get => luck; set => luck = value; }        // Управление параметром удачи
    public bool IsGodMode { get => isGodMode; set => isGodMode = value; } // Управление режимом бога
    public int DropRadius { get => dropRadius; set => dropRadius = value; } // Управление радиусом подбора

    // Переопределенный метод инициализации из справочника
    // Дополняет базовую инициализацию специфичными для героя параметрами
    public override void InitializeFromDictionary(DictionaryCharacters dictionary)
    {
        base.InitializeFromDictionary(dictionary); // Вызываем базовую инициализацию характеристик

        // Дополнительная инициализация параметра удачи
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            if (item.Code == CharacterStatCode.Luck)
            {
                Luck = (int)item.BaseAmount; // Устанавливаем базовое значение удачи
            }
        }
    }
}