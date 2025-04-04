using System;
using System.Collections.Generic;
using System.Linq;

// Статический класс DotsDictionary - справочник эффектов Damage over Time (DoT) в игре
// Содержит параметры всех типов периодических уронов и их характеристики
public static class DotsDictionary
{
    // Генератор случайных чисел для выбора случайного эффекта
    private static readonly Random random = new Random();

    // Словарь всех DoT-эффектов в игре:
    // Ключ - код эффекта (DotCode), значение - параметры эффекта (ItemDot)
    // Формат: 
    private static readonly Dictionary<DotCode, ItemDot> ITEM_DOTS = new()
    {   
        // { Код, new ItemDot }     Код,            Название, Шаг улучшения урона, Шаг улучшения длительности, Базовый урон, Базовая длительность, Цель, Тип, Улучшаемость
        { DotCode.Fire,   CreateDot(DotCode.Fire,   "Огонь",    1, 1, 1, 5, CharacterStatCode.ActualHP, TypeOfDots.TYPE_FIXED, true) },
        { DotCode.Poison, CreateDot(DotCode.Poison, "Яд",       2, 2, 2, 2, CharacterStatCode.ActualHP, TypeOfDots.TYPE_FIXED, true) }
    };

    // Вспомогательный метод для создания ItemDot с читаемым форматом
    // Позволяет наглядно видеть все параметры при создании нового эффекта
    private static ItemDot CreateDot(DotCode code, string nameRu, int upgradeDotDmgX, int upgradeDotDurX,
                                     int baseDotDmg, int baseDotDuration, CharacterStatCode affectedChar,
                                     TypeOfDots type, bool upgradable)
    {
        return new ItemDot(
            code,               // Уникальный идентификатор эффекта
            nameRu,             // Локализованное название
            upgradeDotDmgX,     // Шаг увеличения урона при улучшении
            upgradeDotDurX,     // Шаг увеличения длительности при улучшении
            baseDotDmg,         // Базовый урон эффекта
            baseDotDuration,    // Базовая длительность эффекта (в тиках)
            affectedChar,       // Характеристика, на которую влияет эффект
            type,               // Тип эффекта (фиксированный/процентный и т.д.)
            upgradable          // Можно ли улучшать этот эффект
        );
    }

    // Получение параметров конкретного эффекта по его коду
    public static ItemDot GetDot(DotCode code)
    {
        // Возвращает параметры эффекта или null, если эффект не найден
        return ITEM_DOTS.TryGetValue(code, out var item) ? item : null;
    }

    // Получение списка всех DoT-эффектов в игре
    public static List<ItemDot> GetAllDots()
    {
        // Создает новый список на основе значений словаря
        return new List<ItemDot>(ITEM_DOTS.Values);
    }

    // Получение случайного DoT-эффекта
    public static ItemDot GetRandomDot()
    {
        var keys = ITEM_DOTS.Keys.ToList(); // Преобразуем коллекцию ключей в список
        DotCode randomKey = keys[random.Next(keys.Count)]; // Выбираем случайный ключ
        return ITEM_DOTS[randomKey]; // Возвращаем значение по ключу
    }
}