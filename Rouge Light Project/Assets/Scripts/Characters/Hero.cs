using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Hero : Character, IAttacker, IMovable
{
    protected override void Start()
    {
        base.Start();
        isEnemy = false; // Герой не является врагом
    }

    // Реализация IAttacker
    public void Shoot()
    {
        Debug.Log("Герой стреляет с повышенной точностью!");
    }

    // Реализация IMovable
    public void Move()
    {
        Debug.Log("Герой движется со скоростью " + moveSpeed);
    }

}