using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Shooting : MonoBehaviour
{
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
    public void Shot(int baseDmg, float coolDown)
    {
        coolDown = 1 / coolDown;
       
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
            bulletScript.dotsArray = dotsArray;

            // Логика выстрела
            Debug.Log("Выстрел!");
            nextFireTime = Time.time + coolDown; // Устанавливаем время следующего выстрела
        }
    }
    //Присваивание пуле тега в соответствии с тегом стреляющего
    void BulletTag(GameObject bullet, string whoIsShooter)
    {
        bullet.tag = string.Concat(whoIsShooter, "Bullet");
    }
}

