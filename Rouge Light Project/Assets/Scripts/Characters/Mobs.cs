using UnityEngine;

public class Mobs : Character, IAttacker, IMovable
{
    protected override void Start()
    {
        base.Start();
        isEnemy = true; // Моб является врагом
    }

    // Реализация IAttacker
    public void Shoot()
    {
        Debug.Log("Моб стреляет с пониженной точностью!");
    }

    // Реализация IMovable
    public void Move()
    {
        Debug.Log("Моб движется со скоростью " + moveSpeed);
    }
}
