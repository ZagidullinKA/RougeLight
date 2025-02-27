using log4net;
using UnityEditor;
using System;

using UnityEngine;

public class GameGeneration : MonoBehaviour
{
    //ƒобавл€ем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Observer));

    private static float mobsAmount = 3; // первоначальное количество мобо дл€ генерации
    private static float mobsMultipier = 1; // первоначальный множитель характеристик мобов

    private static float minDistance = 10f; // ћинимальное рассто€ние генерации моба
    private static float maxDistance = 20f; // ћаксимальное рассто€ние генерации моба

    public static float MobsAmount => mobsAmount;
    public static float MobsMultipier => mobsMultipier;

    public static void OverestimatingMobsAmount()
    {
        mobsAmount = (float) Math.Round( (mobsAmount * 1.1 ), 2);
        log.Debug(" рива€ сложности количства мобов увеличилась до - " + mobsAmount);
    }

    public static void OverestimatingMobsMultipier()
    {
        mobsMultipier = (float)Math.Round((mobsMultipier * 1.1), 2);
        log.Debug(" рива€ сложности множитель характеристик уувеличилс€ до - " + mobsMultipier);
    }

    public static void GenerationMobs()
    {
        int mobsAmountMoment = (int) Math.Round(mobsAmount);
        log.Debug("√енерируем " + mobsAmountMoment + " мобов, при mobsAmount = " + mobsAmount);
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab");
        if (enemyPrefab != null)
        {
            for (int i = 0; i < mobsAmountMoment; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, GetRandomPositionAroundHero(), Quaternion.identity);
                if (enemy == null)
                {
                    log.Error("√енераци€ не удалась, enemy is null");
                }
            }
        } else
        {
            log.Error("enemyPrefab is null");
        }
    }

    private static Vector3 GetRandomPositionAroundHero()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPosition = player.transform.position;

        // √енераци€ случайного угла (в радианах)
        float randomAngle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);

        // √енераци€ случайного рассто€ни€ в диапазоне [minDistance, maxDistance]
        float randomDistance = UnityEngine.Random.Range(minDistance, maxDistance);

        // ¬ычисление смещени€ относительно геро€
        Vector3 offset = new Vector3(
            Mathf.Cos(randomAngle) * randomDistance,
            Mathf.Sin(randomAngle) * randomDistance,
            0
        );

        // ¬озвращаем позицию относительно геро€
        return playerPosition + offset;
    }
}
