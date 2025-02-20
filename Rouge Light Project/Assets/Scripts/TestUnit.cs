using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using UnityEngine;
using static UnityEditor.Progress;

public class TestUnit : MonoBehaviour
{
    private Rigidbody2D rb;

    public int health = 100; // Здоровье противника
    public float dodgeChance = 0.01f; // Вероятность уклонения (20%)

    // Метод получения урона
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Противник получил урон: " + damage + ". Осталось здоровья: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDots(List<DotEffect> usableDotsArray)
    {
        if (usableDotsArray.Count > 0) { Debug.Log("Там чет есть"); } else { Debug.LogError("Тут ничего нет!"); }
        foreach (var item in usableDotsArray)
        {
            Debug.Log(item.code);
        }
    }

    // Метод проверки вероятности уклонения
    public bool TryDodge()
    {
        float randomValue = Random.value; // Генерация случайного числа от 0 до 1
        Debug.Log(randomValue);
        Debug.Log(randomValue < dodgeChance);
        return randomValue > dodgeChance;
    }

    // Метод смерти противника
    private void Die()
    {
        Debug.Log("Противник умер!");
        // Здесь можно добавить логику для уничтожения объекта или других действий
        Destroy(gameObject);
    }
}