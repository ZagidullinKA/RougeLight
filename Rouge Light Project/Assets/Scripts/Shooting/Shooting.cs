using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

using Vector2 = UnityEngine.Vector2;
using log4net;
using System.Linq;

public class Shooting : MonoBehaviour
{
    //��������� �����������
    private static readonly ILog log = LogManager.GetLogger(typeof(Shooting));

    public GameObject bulletPrefab;
    public Rigidbody2D rb;

    public string whoIsShooter;

    private float critDamageMultiplier = 2;

    private void Start()
    {
        whoIsShooter = gameObject.tag;
    }
    

    // Метод для стрельбы с заданным направлением
    public void Shot(int baseDmg, 
        int critChance, 
        int bulletFlySpeed, 
        int bulletTimeAlive, 
        List<UsableDotEffect> usableDotsArray,
        Vector3 shootPosition,
        Vector2 direction,
        float spread = 0f
        )
    {
        // Вычисляем отклонение направления на основе spread
        Vector2 aimCoords = CalculateSpreadDirection(direction, spread);

        UsableDotsArray(usableDotsArray, baseDmg);

        string layerTag = string.Concat(whoIsShooter, "Bullet"); 
        int LayerIndex = LayerMask.NameToLayer(layerTag);

        GameObject bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);

        BulletTag(bullet, whoIsShooter);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
            
        // Устанавливаем параметры пули
        bulletScript.SetAimCoords(aimCoords);
        bulletScript.SetUsableDotsArray(usableDotsArray);
        bulletScript.SetDamage(DamageCalc(baseDmg, critChance));
        bulletScript.SetBulletFlySpeed(bulletFlySpeed);
        bulletScript.SetBulletTimeAlive(bulletTimeAlive);
        bulletScript.SetLayerIndex(LayerIndex);
        log.Debug($"Layer Tag: {layerTag}, Layer Index: {LayerIndex}");
    }

    float CritChance(float critChance)
    {
        float diceRoll = Random.Range(0, 1);
        if (diceRoll > critChance)
        {
            return critDamageMultiplier; // Критический удар - умножаем урон���� ���� �������� - ���������� ��������� �����
        }
        else
        {
            return 1; // Обычный удар - множитель 1���� ���� �� �������� - ���������� ��������� 1
        }
    }

    int DamageCalc(int BaseDmg, float critChance)
    {
        return (int)Math.Round(BaseDmg * CritChance(critChance));
    }

    //������������ ���� ���� � ������������ � ����� �����������
    void BulletTag(GameObject bullet, string whoIsShooter)
    {
        bullet.tag = string.Concat(whoIsShooter, "Bullet");
    }

    private void UsableDotsArray(List<UsableDotEffect> usableDotsArray, int baseDmg)
    {
        foreach (var dotEffect in usableDotsArray)
        {
            log.Debug($"DoT Effect - Damage: {dotEffect.DotDmg}, Duration: {dotEffect.DotDur}");
            if (dotEffect.DotDmg == 0)
            {
                log.Warn($"DoT Effect {dotEffect} has zero damage");
            }
            if (dotEffect.DotDur == 0)
            {
                log.Warn($"DoT Effect {dotEffect} has zero duration");
            }

            log.Debug($"DoT Effect type: {dotEffect.Type}");
            if (dotEffect.Type == TypeOfDots.TYPE_BASE_DMG_PERCENT)
            {
                log.Debug("��������� " + (float)dotEffect.DotDmg / 100 + " ���������� ���� �� ���������� " + (float)baseDmg * (float)dotEffect.DotDmg / 100 + " ����������� ���� " + (int)MathF.Ceiling((float)baseDmg * (float)dotEffect.DotDmg / 100));
                dotEffect.DotDmg = (int)MathF.Ceiling((float)baseDmg * (float)dotEffect.DotDmg / 100);
            }
        }
    }

    /// <summary>
    /// Вычисляет направление с учетом разброса (spread)
    /// </summary>
    /// <param name="baseDirection">Базовое направление</param>
    /// <param name="spread">Разброс в градусах</param>
    /// <returns>Направление с отклонением</returns>
    private Vector2 CalculateSpreadDirection(Vector2 baseDirection, float spread)
    {
        if (spread <= 0f)
        {
            return baseDirection.normalized;
        }

        // Вычисляем случайное отклонение в диапазоне (-spread, spread)
        float randomSpread = Random.Range(-spread, spread);
        
        // Преобразуем направление в угол, добавляем отклонение и обратно в направление
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
        float spreadAngle = baseAngle + randomSpread;
        
        // Преобразуем обратно в Vector2
        float angleInRadians = spreadAngle * Mathf.Deg2Rad;
        Vector2 spreadDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        
        return spreadDirection.normalized;
    }
}

