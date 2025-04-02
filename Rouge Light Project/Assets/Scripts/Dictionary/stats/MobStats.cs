using UnityEngine;
using System.Collections.Generic;
using System;
using log4net;

[CreateAssetMenu(fileName = "MobStats", menuName = "Character/MobStats")]
public class MobStats : BaseStats
{
    private static readonly ILog log = LogManager.GetLogger(typeof(MobStats));


    [SerializeField] protected int idMob = 0;         // Уникальный идентификатор моба
    [SerializeField] protected int deathPrice = 0;    // Награда за убийство моба

    // Свойства с прямым get и set для специфичных характеристик мобов
    public int IdMob { get => idMob; set => idMob = value; }
    public int DeathPrice { get => deathPrice; set => deathPrice = value; }


    // Инициализация из справочника (дополнена для мобов)
    public override void InitializeFromDictionary(DictionaryCharacters dictionary)
    {
        base.InitializeFromDictionary(dictionary); // Вызываем базовую инициализацию
        // Здесь можно добавить специфичную для мобов инициализацию, если есть данные в справочнике
    }
}