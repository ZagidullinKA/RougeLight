using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using System.Collections.Generic;
using log4net;

public class Bullet : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Bullet));

    private float bulletTimeAlive;
    private float bulletFlySpeed;
    private Rigidbody2D rb;

    private List<DotEffect> usableDotsArray;
    private int damage;
    private Vector2 aimCoords;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        BulletGeneration(aimCoords, usableDotsArray);
    }

    public void BulletGeneration(Vector2 aimCoords, List<DotEffect> usableDotsArray)
    {
        log.Debug("Пуля создана");
        float rotate = Mathf.Atan2(aimCoords.y, aimCoords.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotate);
        rb.linearVelocity = transform.up * bulletFlySpeed;

        Invoke("DestroyBullet", bulletTimeAlive);
    }
   
    public void DestroyBullet()
    {
        Destroy(this.gameObject);
        log.Debug("Пуля уничтожена");
    }

    public int DamageDealing()
    {
        log.Debug("Наносим урон");
        // Туть вызов сеттера урона персонажа
        return damage;
    }

    // Описываем гетеры и сеттеры для характеристик пули
    public void SetDamage(int Damage) { damage = Damage; }

    public int GetDamage() { return damage; }

    public void SetBulletTimeAlive(float timeAlive) { bulletTimeAlive = timeAlive; }

    public void SetBulletFlySpeed(float FlySpeed) { bulletFlySpeed = FlySpeed; }

    public void SetUsableDotsArray(List<DotEffect> DotsArray) { usableDotsArray = DotsArray; }

    public List<DotEffect> GetUsableDotsArray() { return usableDotsArray; }

    public void SetAimCoords(Vector2 Coords) { aimCoords = Coords; }
}
