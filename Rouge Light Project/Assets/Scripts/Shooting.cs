using System;
using UnityEngine;
using System.Collections.Generic;

using Vector2 = UnityEngine.Vector2;
using log4net;

public class Shooting : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Shooting));

    public GameObject bulletPrefab;
    public Rigidbody2D rb;

    public Array dotsArray;
    public string whoIsShooter;

    private float nextFireTime;

    [SerializeField] private GameObject shooter;
    [SerializeField] private GameObject bulletSpawn;

    private void Start()
    {
        whoIsShooter = gameObject.tag;
    }
    public void Shot(int baseDmg, float attackSpeed, List<DotEffect> usableDotsArray)
    {
        float coolDown = 1 / attackSpeed;
       
        if (Time.time >= nextFireTime)
        {
            bulletSpawn = GameObject.FindGameObjectWithTag("PlayerFirePoint");
            Vector2 firePoint = bulletSpawn.transform.position;

            shooter = GameObject.FindGameObjectWithTag("Player");
            Vector2 unitPos = shooter.transform.position;

            Vector2 aimCoords = firePoint - unitPos;

            GameObject bullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);

            BulletTag(bullet, whoIsShooter);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.aimCoords = aimCoords;
            bulletScript.baseDmg = baseDmg;
            bulletScript.usableDotsArray = usableDotsArray;

            // Логика выстрела
            log.Debug("Выстрел!");
            nextFireTime = Time.time + coolDown; // Устанавливаем время следующего выстрела
        }
    }
    //Присваивание пуле тега в соответствии с тегом стреляющего
    void BulletTag(GameObject bullet, string whoIsShooter)
    {
        bullet.tag = string.Concat(whoIsShooter, "Bullet");
    }
}

