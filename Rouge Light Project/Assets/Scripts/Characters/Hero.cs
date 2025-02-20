using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Hero : Character, IAttacker, IMovable
{
    public Rigidbody2D rb;
    public Shooting shooting;
    public Transform firePoint;
    public GameObject Bullet;
    private Vector2 moveVector;
    private bool isShooting = false;

    protected override void Start()
    {
        // Заглушка ебаная
        usableDotsArray.Add(new DotEffect("fire1", 1, 1, 1, 1));
        // Конец заглушки ебаной

        Debug.Log(usableDotsArray[0].code);
        base.Start();
        isEnemy = false; // Герой не является врагом
        InitializeCharacteristics();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shooting = GetComponent<Shooting>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            isShooting = true;
        }
    }

    private void FixedUpdate()
    {
        Move();
        Shoot();
    }



    // Реализация IAttacker
    public void Shoot()
    {
        if (isShooting)
        {
            shooting.Shot((int) dmg, (float) atkSpeed, usableDotsArray);
            isShooting = false;
        }
    }

    // Реализация IMovable
    public void Move()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");

        rb.MovePosition(rb.position + moveVector * moveSpeed * Time.deltaTime);
        RotateTowardsMouse();
    }

    // Инициализация характеристик
    private void InitializeCharacteristics()
    {
        // Используем справочник всех характеристик
        foreach (var item in DictionaryCharacters.GetAllCharacteristics())
        {
            Debug.Log("foreachCharacteristic :" + item.Code);
            if (item.Upgradable)
            {
                // Получаем улучшаемые характеристики из справочника улучшаемых характеристик
                var upgradableItem = ImprovableCharactesDictionary.GetAllImprovableCharacteristics().Find(x => x.Code == item.Code);
                if (upgradableItem != null)
                {
                    SetCharacteristic(item.Code, upgradableItem.FinalValue);
                }
            }
            else
            {
                // Используем базовые значения из справочника всех характеристик
                SetCharacteristic(item.Code, item.BaseAmount);
            }
        }
    }

    // Установка значения характеристики
    private void SetCharacteristic(string code, int? value)
    {
        Debug.Log("SetCharacteristic :" + code + " - " + value);
        switch (code)
        {
            case "maxHP":
                maxHP = (int)value;
                break;
            case "dmg":
                dmg = (int)value;
                break;
            case "atkSpeed":
                atkSpeed = (int)value;
                break;
            case "moveSpeed":
                moveSpeed = (int) value;
                break;
            case "luck":
                luck = (int)value;
                break;
            case "critChance":
                critChance = (int)value;
                break;
            case "evadeChace":
                evadeChance = (int)value;
                break;
            case "armor":
                armor = (int)value;
                break;
            case "debuffResist":
                debuffResist = (int) value;
                break;
            case "Vampire":
                vampire = (int)value;
                break;
            case "hpFromDropRestore":
                hpFromDropRestore = (int)value;
                break;
            case "dropRadius":
                dropRadius = (int)value;
                break;
            default:
                Debug.LogWarning($"Неизвестная характеристика: {code}");
                break;
        }
    }

    void RotateTowardsMouse()
    {
        // Получаем позицию курсора в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Вычисляем направление от игрока к курсору
        Vector2 direction = mousePosition - transform.position;

        // Вычисляем угол для поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Устанавливаем новый угол поворота
        rb.rotation = angle;
    }

}