using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class Bullet : MonoBehaviour
{
    public float timeDestroy = 3f;
    public float speed = 3f;
    public Rigidbody2D rb;

    public int baseDmg = 1;
    public Array dotsArray;
    public int damage;
    public Vector2 aimCoords;

    [SerializeField] private GameObject shooter;
    [SerializeField] private GameObject bulletSpawn;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        damage = DamageCalc(baseDmg);

        BulletGeneration(aimCoords, baseDmg, dotsArray);
    }

    public void BulletGeneration(Vector2 aimCoords, int baseDmg, Array dotsArray)
    {
        shooter = GameObject.FindGameObjectWithTag("Player");
        Vector2 unitPos = shooter.transform.position;

        bulletSpawn = GameObject.FindGameObjectWithTag("PlayerFirePoint");
        Vector2 firePoint = bulletSpawn.transform.position;
        aimCoords = firePoint - unitPos;

        float rotate = Mathf.Atan2(aimCoords.y, aimCoords.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotate);
        rb.linearVelocity = transform.up * speed;

        Invoke("DestroyBullet", timeDestroy);
    }

    int DamageCalc(int baseDmg)
    {
        int damage = baseDmg;
        return damage;
    }
   
    void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}
