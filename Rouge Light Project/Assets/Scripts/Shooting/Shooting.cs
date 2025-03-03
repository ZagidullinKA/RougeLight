using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

using Vector2 = UnityEngine.Vector2;
using log4net;

public class Shooting : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Shooting));

    public GameObject bulletPrefab;
    public Rigidbody2D rb;

    public string whoIsShooter;

    private float nextFireTime;

    private float critDamageMultiplier = 2;

    [SerializeField] private GameObject shooter;
    [SerializeField] private GameObject bulletSpawn;

    private void Start()
    {
        whoIsShooter = gameObject.tag;
    }
    public void Shot(int baseDmg, 
        int critChance, 
        float? attackSpeed, 
        int bulletFlySpeed, 
        int bulletTimeAlive, 
        List<DotEffect> usableDotsArray)
    {
        float coolDown = 1 / (float) attackSpeed; // устанавливаем задержку стрельбы
        log.Debug("Кулдаун " + coolDown);
       
        if (Time.time >= nextFireTime)
        {
            bulletSpawn = GameObject.FindGameObjectWithTag("PlayerFirePoint");
            Vector2 firePoint = bulletSpawn.transform.position;

            shooter = GameObject.FindGameObjectWithTag("Player");
            Vector2 unitPos = shooter.transform.position;

            Vector2 aimCoords = firePoint - unitPos;

            UsableDotsArray(usableDotsArray, baseDmg);

            GameObject bullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);

            BulletTag(bullet, whoIsShooter);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            
            // Передаем пуле характеристики
            bulletScript.SetAimCoords(aimCoords);
            bulletScript.SetUsableDotsArray(usableDotsArray);
            bulletScript.SetDamage(DamageCalc(baseDmg, critChance));
            bulletScript.SetBulletFlySpeed(bulletFlySpeed);
            bulletScript.SetBulletTimeAlive(bulletTimeAlive);

            nextFireTime = Time.time + coolDown; // Устанавливаем время следующего выстрела
            log.Debug("nexyFireTime " +  nextFireTime + " Time.time " + Time.time);
        }
    }

    float CritChance(float critChance)
    {
        float diceRoll = Random.Range(0, 1);
        if (diceRoll > critChance)
        {
            return critDamageMultiplier; // Если крит сработал - возвращаем множитель крита
        }
        else
        {
            return 1; // Если крит не сработал - возвращаем множитель 1
        }
    }

    int DamageCalc(int BaseDmg, float critChance)
    {
        return (int)Math.Round(BaseDmg * CritChance(critChance));
        
        // хуяк=хуяк и в коммит
    }

    //Присваивание пуле тега в соответствии с тегом стреляющего
    void BulletTag(GameObject bullet, string whoIsShooter)
    {
        bullet.tag = string.Concat(whoIsShooter, "Bullet");
    }

    private void UsableDotsArray(List<DotEffect> usableDotsArray, int baseDmg)
    {
        foreach (var dotEffect in usableDotsArray)
        {
            log.Debug("dotEffect.DotDmg is " +  dotEffect.DotDmg + 
                " and dotEffect.DotDur is " + dotEffect.DotDur);
            if (dotEffect.DotDmg == 0)
            {
                log.Warn("In " + dotEffect + " DotDmg is 0");
            }
            if (dotEffect.DotDur == 0)
            {
                log.Warn("In " + dotEffect + " DotDur is 0");
            }
            if (dotEffect.type == "baseDmgPercent")
            {
                dotEffect.finalDotDmg = baseDmg*(int)Math.Ceiling(dotEffect.finalDotDmg/100);
            }
        }
    }
}

