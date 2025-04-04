// Импорт необходимых пространств имен
using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion; // Явное указание пространства имен для Quaternion
using Vector2 = UnityEngine.Vector2;      // Явное указание пространства имен для Vector2
using System.Collections.Generic;         // Для работы с коллекциями (List)
using log4net;                            // Для системы логирования

// Класс Bullet управляет поведением пули в игре
public class Bullet : MonoBehaviour
{
    //Добавляем логирование
    // Инициализация логгера для этого класса
    private static readonly ILog log = LogManager.GetLogger(typeof(Bullet));

    // Параметры пули
    private float bulletTimeAlive;    // Время жизни пули в секундах
    private float bulletFlySpeed;     // Скорость полета пули
    private Rigidbody2D rb;           // Ссылка на компонент Rigidbody2D

    // Характеристики пули
    private List<UsableDotEffect> usableDotsArray; // Список DOT-эффектов, которые наносит пуля
    private int damage;              // Урон, наносимый пулей
    private Vector2 aimCoords;       // Направление полета пули
    private int layerIndex;          // Индекс слоя для коллизий

    // Метод Start вызывается при инициализации объекта
    void Start()
    {
        // Получаем компонент Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Инициализируем пулю с заданными параметрами
        BulletGeneration(aimCoords, usableDotsArray);
    }

    // Основной метод инициализации и запуска пули
    public void BulletGeneration(Vector2 aimCoords, List<UsableDotEffect> usableDotsArray)
    {
        log.Debug("Пуля создана");

        // Рассчитываем угол поворота пули по направлению движения
        float rotate = Mathf.Atan2(aimCoords.y, aimCoords.x) * Mathf.Rad2Deg - 90f;

        // Применяем поворот к пуле
        transform.rotation = Quaternion.Euler(0f, 0f, rotate);

        // Задаем скорость движения пули в направлении "вверх" от текущего поворота
        rb.linearVelocity = transform.up * bulletFlySpeed;

        // Устанавливаем слой для коллизий
        gameObject.layer = layerIndex;

        // Запускаем таймер самоуничтожения пули
        Invoke("DestroyBullet", bulletTimeAlive);
    }

    // Метод уничтожения пули
    public void DestroyBullet()
    {
        // Уничтожаем объект пули
        Destroy(this.gameObject);
        log.Debug("Пуля уничтожена");
    }

    // Метод получения урона пули
    public int DamageDealing()
    {
        log.Debug("Наносим урон");
        // Туть вызов сеттера урона персонажа
        return damage;
    }

    // Группа методов для установки и получения параметров пули

    // Установка урона
    public void SetDamage(int Damage) { damage = Damage; }

    // Получение урона
    public int GetDamage() { return damage; }

    // Установка времени жизни пули
    public void SetBulletTimeAlive(float timeAlive) { bulletTimeAlive = timeAlive; }

    // Установка скорости полета пули
    public void SetBulletFlySpeed(float flySpeed) { bulletFlySpeed = flySpeed; }

    // Установка списка DOT-эффектов
    public void SetUsableDotsArray(List<UsableDotEffect> dotsArray) { usableDotsArray = dotsArray; }

    // Получение списка DOT-эффектов
    public List<UsableDotEffect> GetUsableDotsArray() { return usableDotsArray; }

    // Установка направления полета
    public void SetAimCoords(Vector2 coords) { aimCoords = coords; }

    // Установка слоя для коллизий
    public void SetLayerIndex(int setLayerIndex) { layerIndex = setLayerIndex; }

}
