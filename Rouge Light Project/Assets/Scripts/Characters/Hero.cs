using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Hero : Character, IAttacker, IMovable
{
    protected override void Start()
    {
        base.Start();
        isEnemy = false; // Герой не является врагом
        InitializeCharacteristics();
    }

    // Реализация IAttacker
    public void Shoot()
    {
        Debug.Log("Герой стреляет с повышенной точностью!");
    }

    // Реализация IMovable
    public void Move()
    {
        Debug.Log("Герой движется со скоростью " + moveSpeed);
    }

    // Инициализация характеристик
    private void InitializeCharacteristics()
    {
        // Используем справочник всех характеристик
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            Debug.Log("foreachCharacteristic :" + item.Code);
            if (item.Upgradable)
            {
                // Получаем улучшаемые характеристики из справочника улучшаемых характеристик
                var upgradableItem = ImprovableCharactesDictionary.GetAllImprovableCharacteristics().Find(x => x.Code == item.Code);
                if (upgradableItem != null)
                {
                    SetCharacteristic(item.Code, upgradableItem.FinalValue);
                }
            }
            else
            {
                // Используем базовые значения из справочника всех характеристик
                SetCharacteristic(item.Code, item.BaseAmount);
            }
        }
    }

    // Установка значения характеристики
    private void SetCharacteristic(string code, int? value)
    {
        Debug.Log("SetCharacteristic :" + code + " - " + value);
        switch (code)
        {
            case "maxHP":
                maxHP = value;
                break;
            case "dmg":
                dmg = value;
                break;
            case "atkSpeed":
                atkSpeed = value;
                break;
            case "moveSpeed":
                moveSpeed = value;
                break;
            case "luck":
                luck = value;
                break;
            case "critChance":
                critChance = value;
                break;
            case "evadeChace":
                evadeChance = value;
                break;
            case "armor":
                armor = value;
                break;
            case "debuffResist":
                debuffResist = value;
                break;
            case "Vampire":
                vampire = value;
                break;
            case "hpFromDropRestore":
                hpFromDropRestore = value;
                break;
            case "dropRadius":
                dropRadius = value;
                break;
            default:
                Debug.LogWarning($"Неизвестная характеристика: {code}");
                break;
        }
    }

}