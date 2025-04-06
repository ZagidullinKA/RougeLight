// Импорт необходимых пространств имен
using log4net;         // Для системы логирования
using UnityEditor;     // Для работы с редактором Unity (AssetDatabase)
using System;          // Базовые типы .NET
using UnityEngine;     // Базовые функции Unity

// Класс GameGeneration отвечает за генерацию мобов и управление сложностью игры
public class GameGeneration : MonoBehaviour
{
    //Добавляем логирование
    // Инициализация логгера (используется логгер от Observer для единообразия)
    private static readonly ILog log = LogManager.GetLogger(typeof(Observer));

    // Настройки генерации мобов
    private static float mobsAmount = 5; // первоначальное количество мобо для генерации
    private static float mobsMultiplier = 1; // первоначальный множитель характеристик мобов

    private static int mobIterationNumber = 0; // Счетчик для уникальных ID мобов

    // Настройки позиционирования мобов
    private static float minDistance = 10f; // Минимальное расстояние генерации моба от игрока
    private static float maxDistance = 20f; // Максимальное расстояние генерации моба от игрока

    // Публичные свойства для доступа к приватным переменным
    public static float MobsAmount => mobsAmount; // Текущее количество мобов для генерации
    public static float MobsMultipier => mobsMultiplier; // Текущий множитель характеристик мобов

    // Метод для увеличения количества генерируемых мобов (увеличение сложности)
    public static void OverestimatingMobsAmount()
    {
        // Увеличиваем количество на 10% и округляем до 2 знаков
        mobsAmount = (float)Math.Round((mobsAmount * 1.1), 2);
        log.Debug("Кривая сложности количства мобов увеличилась до - " + mobsAmount);
    }

    // Метод для увеличения множителя характеристик мобов (увеличение сложности)
    public static void OverestimatingMobsMultipier()
    {
        // Увеличиваем множитель на 10% и округляем до 2 знаков
        mobsMultiplier = (float)Math.Round((mobsMultiplier * 1.1), 2);
        log.Debug("Кривая сложности множитель характеристик уувеличился до - " + mobsMultiplier);
    }

    // Основной метод генерации мобов
    public static void GenerationMobs()
    {
        // Округляем количество мобов до целого числа
        int mobsAmountMoment = (int)Math.Round(mobsAmount);
        log.Debug("Генерируем " + mobsAmountMoment + " мобов, при mobsAmount = " + mobsAmount);

        // Загрузка префаба моба из Assets
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab");

        if (enemyPrefab != null)
        {
            // Генерация указанного количества мобов
            for (int i = 0; i < mobsAmountMoment; i++)
            {
                // Создание экземпляра моба в случайной позиции вокруг героя
                GameObject enemy = Instantiate(enemyPrefab, GetRandomPositionAroundHero(), Quaternion.identity);

                // Назначение уникального ID мобу
                Mobs enemyScript = enemy.GetComponent<Mobs>();
                enemyScript.mobStats.IdMob = mobIterationNumber;
                mobIterationNumber++;

                // Проверка на успешное создание моба
                if (enemy == null)
                {
                    log.Error("Генерация не удалась, enemy is null");
                }
            }
        }
        else
        {
            log.Error("enemyPrefab is null");
        }
    }

    // Метод для получения случайной позиции вокруг героя
    private static Vector3 GetRandomPositionAroundHero()
    {
        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPosition = player.transform.position;

        // Генерация случайного угла (в радианах)
        float randomAngle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);

        // Генерация случайного расстояния в диапазоне [minDistance, maxDistance]
        float randomDistance = UnityEngine.Random.Range(minDistance, maxDistance);

        // Вычисление смещения относительно героя
        Vector3 offset = new Vector3(
            Mathf.Cos(randomAngle) * randomDistance, // X координата
            Mathf.Sin(randomAngle) * randomDistance, // Y координата
            0                                       // Z координата (2D игра)
        );

        // Возвращаем позицию относительно героя
        return playerPosition + offset;
    }
}