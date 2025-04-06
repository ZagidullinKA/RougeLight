using UnityEngine;
using System.Collections.Generic;
using System;
using log4net;

// Класс MobStats наследуется от BaseStats и содержит специфичные для мобов параметры
// Представляет ScriptableObject для настройки характеристик врагов через редактор Unity
[CreateAssetMenu(fileName = "MobStats", menuName = "Character/MobStats")]
public class MobStats : BaseStats
{
    // Инициализация логгера для записи событий и ошибок
    private static readonly ILog log = LogManager.GetLogger(typeof(MobStats));

    // Уникальный идентификатор моба:
    // - Используется для связи с другими системами
    // - Позволяет различать типы мобов
    [SerializeField] protected int idMob = 0;         // Уникальный идентификатор моба

    // Награда за убийство моба:
    // - Количество очков/валюты за убийство
    // - Может варьироваться в зависимости от сложности моба
    [SerializeField] protected int deathPrice = 0;    // Награда за убийство моба

    // Свойства с прямым get и set для специфичных характеристик мобов
    public int IdMob { get => idMob; set => idMob = value; }
    public int DeathPrice { get => deathPrice; set => deathPrice = value; }

    // Переопределенный метод инициализации из справочника
    // Дополняет базовую инициализацию специфичными для мобов параметрами
    public override void InitializeFromDictionary(DictionaryCharacters dictionary)
    {
        base.InitializeFromDictionary(dictionary); // Вызываем базовую инициализацию
        // Здесь можно добавить специфичную для мобов инициализацию, если есть данные в справочнике
    }
}