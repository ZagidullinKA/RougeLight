using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    public float timeDestroy = 3f;
    public float speed = 3f;
    private Rigidbody2D rb;

    public int baseDmg = 1;
    public Array dotsArray;
    public int damage;
    public Vector2 aimCoords;

    public float critDamageProbability;
    public float critDamageMultiplier;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        DamageCalc();

        BulletGeneration(aimCoords, baseDmg, dotsArray);
    }

    public void BulletGeneration(Vector2 aimCoords, int baseDmg, Array dotsArray)
    {
        float rotate = Mathf.Atan2(aimCoords.y, aimCoords.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotate);
        rb.linearVelocity = transform.up * speed;

        Invoke("DestroyBullet", timeDestroy);
    }

    float CritChance(float critDamageProbability)
    {
        float diceRoll = Random.Range(0, 1);
        if (diceRoll > critDamageProbability) 
        {
            return critDamageMultiplier; // Если крит сработал - возвращаем множитель крита
        }
        else
        {
            return 1; // Если крит не сработал - возвращаем множитель 1
        }
    }

    int DamageCalc()
    {
        damage = (int)Math.Round(baseDmg*CritChance(critDamageProbability));
        return damage;
    }
   
    public void DestroyBullet()
    {
        Destroy(this.gameObject);
    }

    public void DamageDealing()
    {
        Debug.Log("Наносим урон");
        // Туть вызов сеттера урона персонажа
    }
}
