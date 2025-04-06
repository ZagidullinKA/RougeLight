using System;
using UnityEngine;

// Класс ValidationValue предоставляет методы для валидации различных типов данных
// Используется для проверки входных параметров на корректность
public static class ValidationValue
{
    // Проверяет, что строковые значения не являются null или пустыми
    // Параметры:
    // variables - массив кортежей (значение, имя переменной) для проверки
    public static void ValidateStringNotNullOrEmpty(params (string value, string name)[] variables)
    {
        foreach (var (value, name) in variables)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{name} не может быть пустым", name);
            }
        }
    }

    // Проверяет, что целочисленные значения не являются null
    // Параметры:
    // variables - массив кортежей (значение, имя переменной) для проверки
    public static void ValidateIntNotNull(params (int? value, string name)[] variables)
    {
        foreach (var (value, name) in variables)
        {
            if (value == null)
            {
                throw new ArgumentException($"{name} не может быть пустым", name);
            }
        }
    }

    // Проверяет, что значения с плавающей точкой не являются null
    // Параметры:
    // variables - массив кортежей (значение, имя переменной) для проверки
    public static void ValidateFloatNotNull(params (float? value, string name)[] variables)
    {
        foreach (var (value, name) in variables)
        {
            if (value == null)
            {
                throw new ArgumentException($"{name} не может быть пустым", name);
            }
        }
    }
}