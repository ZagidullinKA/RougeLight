using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Shoooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Rigidbody2D rb;

    public int baseDmg = 1;
    public Array dotsArray;

    public string whoIsShooter;

    [SerializeField] private GameObject shooter;
    [SerializeField] private GameObject bulletSpawn;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bulletSpawn = GameObject.FindGameObjectWithTag("PlayerFirePoint");
            Vector2 firePoint = bulletSpawn.transform.position;

            GameObject bullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);

            // Присваиваем тег пуле
            if (whoIsShooter == "Player")
            {
                bullet.tag = "playerBullet";
            }
            if (whoIsShooter == "Enemy")
            {
                bullet.tag = "enemyBullet"; 
            }
            else
            {
                bullet.tag = "Player";
            }

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.baseDmg = baseDmg;
            bulletScript.dotsArray = dotsArray;

           // bullet.BulletGeneration(aimCoords, baseDmg, dotsArray);
        }
    }
}
