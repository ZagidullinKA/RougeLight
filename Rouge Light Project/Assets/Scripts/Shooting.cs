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

    private void Start()
    {
        whoIsShooter = gameObject.tag;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
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
        }
    }

    //Присваивание пуле тега в соответствии с тегом стреляющего
    void BulletTag(GameObject bullet, string whoIsShooter)
    {
        bullet.tag = string.Concat(whoIsShooter, "Bullet");
    }
}

