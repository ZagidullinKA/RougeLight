using System.Collections.Generic;
using System.Linq;
using log4net;
using Mono.Cecil.Cil;
using NUnit.Framework;
using UnityEngine;
using static UnityEditor.Progress;

public class TestUnit : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(TestUnit));

    private Rigidbody2D rb;

    public int health = 100; // Здоровье противника
    public float dodgeChance = 0.01f; // Вероятность уклонения (20%)

    // Метод получения урона
    public void TakeDamage(int damage)
    {
        health -= damage;
        log.Debug("Противник получил урон: " + damage + ". Осталось здоровья: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDots(List<DotEffect> usableDotsArray)
    {
        if (usableDotsArray.Count > 0) { log.Debug("Там чет есть"); } else { log.Error("Тут ничего нет!"); }
        foreach (var item in usableDotsArray)
        {
            log.Debug(item.code);
        }
    }

    // Метод проверки вероятности уклонения
    public bool TryDodge()
    {
        float randomValue = Random.value; // Генерация случайного числа от 0 до 1
        log.Debug(randomValue);
        log.Debug(randomValue < dodgeChance);
        return randomValue > dodgeChance;
    }

    // Метод смерти противника
    private void Die()
    {
        log.Debug("Противник умер!");
        // Здесь можно добавить логику для уничтожения объекта или других действий
        Destroy(gameObject);
    }
}