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

    // ===== НОВЫЙ КОД ===========================================================================================================
    [System.Serializable]
    public class OutlineSettings
    {
        [Tooltip("Цвет окантовки пули")]
        public Color color = Color.red;

        [Tooltip("Толщина окантовки в пикселях")]
        [Range(1, 50)]
        public float size = 3f;

        [Tooltip("Видимость окантовки")]
        public bool enabled = true;
    }

    [Header("Настройки окантовки")]
    [SerializeField] private OutlineSettings outlineSettings = new OutlineSettings();
    // ===== КОНЕЦ НОВОГО КОДА =====================================================================================================


    // Параметры пули
    private float bulletTimeAlive;    // Время жизни пули в секундах
    private float bulletFlySpeed;     // Скорость полета пули
    private Rigidbody2D rb;           // Ссылка на компонент Rigidbody2D

    // Характеристики пули
    private List<UsableDotEffect> usableDotsArray; // Список DOT-эффектов, которые наносит пуля
    private int damage;              // Урон, наносимый пулей
    private Vector2 aimCoords;       // Направление полета пули
    private int layerIndex;          // Индекс слоя для коллизий

    // ===== НОВЫЙ КОД =============================================================================================================
    // Константы для окантовки
    private const float OUTLINE_SIZE = 3f; // Толщина окантовки в пикселях
    private const string OUTLINE_SORTING_LAYER = "Default"; // Слой сортировки
    private const int OUTLINE_ORDER_IN_LAYER = -1; // Окантовка будет позади основной пули

    /// <summary>
    /// Добавляет красную окантовку к пуле
    /// </summary>
    private void AddOutline()
    {
        if (!outlineSettings.enabled) return;

        // Получаем основной SpriteRenderer пули
        SpriteRenderer mainRenderer = GetComponentInChildren<SpriteRenderer>();

        if (mainRenderer == null)
        {
            log.Warn("Не найден SpriteRenderer для добавления окантовки");
            return;
        }

        // Создаем новый GameObject для окантовки
        GameObject outlineObject = new GameObject("BulletOutline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one * (1 + outlineSettings.size / 100f); // Увеличиваем размер

        // Добавляем SpriteRenderer для окантовки
        SpriteRenderer outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();

        // Настраиваем параметры окантовки
        outlineRenderer.sprite = mainRenderer.sprite; // Используем тот же спрайт
        outlineRenderer.color = outlineSettings.color; // Настройка цвета окантовки
        outlineRenderer.sortingLayerName = OUTLINE_SORTING_LAYER;
        outlineRenderer.sortingOrder = OUTLINE_ORDER_IN_LAYER; // Окантовка позади основной пули

        log.Debug("Добавлена красная окантовка к пуле");
    }
    // ===== КОНЕЦ НОВОГО КОДА ===============================================================================================================

    // Метод Start вызывается при инициализации объекта
    void Start()
    {
        // Получаем компонент Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // ===== НОВЫЙ КОД =====
        // Добавляем окантовку при создании пули
        AddOutline();
        // ===== КОНЕЦ НОВОГО КОДА =====

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
