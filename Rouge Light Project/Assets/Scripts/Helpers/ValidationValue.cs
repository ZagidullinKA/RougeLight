using System;
using UnityEngine;

public static class ValidationValue
{
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
}
