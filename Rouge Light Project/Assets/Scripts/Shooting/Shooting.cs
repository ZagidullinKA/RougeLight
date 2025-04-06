// Импорт необходимых пространств имен
using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using log4net;
using System.Linq;

// Класс Shooting отвечает за логику стрельбы персонажей и врагов
public class Shooting : MonoBehaviour
{
    // Инициализация логгера для этого класса
    private static readonly ILog log = LogManager.GetLogger(typeof(Shooting));

    // Публичные поля для настройки стрельбы
    public GameObject bulletPrefab; // Префаб пули
    public Rigidbody2D rb; // Компонент Rigidbody2D стреляющего объекта
    public string whoIsShooter; // Тег объекта, который стреляет (Player/Enemy)

    private float nextFireTime; // Время следующего выстрела
    private float critDamageMultiplier = 2; // Множитель критического урона
    [SerializeField] private GameObject bulletSpawn; // Точка спавна пуль

    // Метод Start вызывается при инициализации объекта
    private void Start()
    {
        // Запоминаем тег объекта для идентификации пуль
        whoIsShooter = gameObject.tag;
    }

    // ===== НОВЫЙ КОД =========================================================================================
    /// <summary>
    /// Универсальный метод определения цвета стреляющего объекта
    /// </summary>
    /// <returns>
    /// Возвращает Color структуру, содержащую RGBA-значение цвета:
    /// - Цвет SpriteRenderer'а на основном объекте (если есть)
    /// - Цвет первого найденного SpriteRenderer'а в дочерних объектах (если есть)
    /// - Белый цвет (Color.white) если SpriteRenderer не найден
    /// </returns>
    private Color GetShooterColor()
    {
        // Сначала проверяем SpriteRenderer на самом объекте
        SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();

        if (mainRenderer != null)
        {
            log.Debug($"Найден SpriteRenderer на основном объекте. Цвет: {mainRenderer.color}");
            return mainRenderer.color;
        }

        // Если на основном объекте нет SpriteRenderer'а, ищем в дочерних объектах
        SpriteRenderer childRenderer = GetComponentInChildren<SpriteRenderer>();

        if (childRenderer != null)
        {
            log.Debug($"Найден SpriteRenderer в дочернем объекте. Цвет: {childRenderer.color}");
            return childRenderer.color;
        }

        log.Warn("Не удалось найти SpriteRenderer ни на основном объекте, ни в дочерних. Используется белый цвет.");
        return Color.white;
    }
    // ===== КОНЕЦ НОВОГО КОДА =================================================================================

    // Основной метод стрельбы
    public void Shot(int baseDmg,
        int critChance,
        float? attackSpeed,
        int bulletFlySpeed,
        int bulletTimeAlive,
        List<UsableDotEffect> usableDotsArray)
    {
        // Рассчитываем кулдаун между выстрелами
        float coolDown = 1 / (float)attackSpeed;

        // Проверка наличия точки спавна пуль
        if (bulletSpawn == null)
        {
            log.Error("bulletSpawn не назначен, выстрел невозможен.");
            return;
        }

        // Проверяем, можно ли стрелять (прошел ли кулдаун)
        if (Time.time >= nextFireTime)
        {
            // Получаем позицию точки выстрела и позицию стрелка
            Vector2 firePoint = bulletSpawn.transform.position;
            Vector2 unitPos = transform.position;

            // Рассчитываем направление выстрела
            Vector2 aimCoords = firePoint - unitPos;
            aimCoords.Normalize();

            // Обрабатываем эффекты DOT (Damage Over Time)
            UsableDotsArray(usableDotsArray, baseDmg);

            // Создаем тег и слой для пули в зависимости от стрелка
            string layerTag = string.Concat(whoIsShooter, "Bullet");
            int LayerIndex = LayerMask.NameToLayer(layerTag);

            // Создаем экземпляр пули
            GameObject bullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);

            // ===== НОВЫЙ КОД ========================================================================
            // Получаем компонент SpriteRenderer у пули
            SpriteRenderer bulletRenderer = bullet.GetComponentInChildren<SpriteRenderer>();

            if (bulletRenderer != null)
            {
                // Устанавливаем цвет пули в соответствии с цветом стреляющего объекта
                bulletRenderer.color = GetShooterColor();
                log.Debug($"Пуля получила цвет: {bulletRenderer.color} (Стреляющий объект: {gameObject.name})");
            }
            else
            {
                log.Warn("Не удалось найти SpriteRenderer у созданной пули");
            }
            // ===== КОНЕЦ НОВОГО КОДА ========================================================================

            // Назначаем пуле соответствующий тег
            BulletTag(bullet, whoIsShooter);

            // Получаем компонент Bullet и настраиваем параметры пули
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            // Передаем пуле характеристики
            bulletScript.SetAimCoords(aimCoords); // Направление полета
            bulletScript.SetUsableDotsArray(usableDotsArray); // DOT-эффекты
            bulletScript.SetDamage(DamageCalc(baseDmg, critChance)); // Урон (с учетом крита)
            bulletScript.SetBulletFlySpeed(bulletFlySpeed); // Скорость полета
            bulletScript.SetBulletTimeAlive(bulletTimeAlive); // Время жизни пули
            bulletScript.SetLayerIndex(LayerIndex); // Слой для коллизий

            // Устанавливаем время следующего выстрела
            nextFireTime = Time.time + coolDown;
        }
    }

    // Метод расчета шанса критического удара
    float CritChance(float critChance)
    {
        // Генерируем случайное число для проверки крита
        float diceRoll = Random.Range(0, 1);
        if (diceRoll > critChance)
        {
            return critDamageMultiplier; // Если крит сработал - возвращаем множитель крита
        }
        else
        {
            return 1; // Если крит не сработал - возвращаем множитель 1
        }
    }

    // Метод расчета урона с учетом крита
    int DamageCalc(int BaseDmg, float critChance)
    {
        return (int)Math.Round(BaseDmg * CritChance(critChance));
    }

    // Метод для назначения тега пуле
    void BulletTag(GameObject bullet, string whoIsShooter)
    {
        bullet.tag = string.Concat(whoIsShooter, "Bullet");
    }

    // Метод обработки DOT-эффектов
    private void UsableDotsArray(List<UsableDotEffect> usableDotsArray, int baseDmg)
    {
        // Перебираем все DOT-эффекты
        foreach (var dotEffect in usableDotsArray)
        {
            // Проверка нулевого урона DOT
            if (dotEffect.DotDmg == 0)
            {
                log.Warn("In " + dotEffect + " DotDmg is 0");
            }
            // Проверка нулевой длительности DOT
            if (dotEffect.DotDur == 0)
            {
                log.Warn("In " + dotEffect + " DotDur is 0");
            }

            // Обработка DOT-эффектов, зависящих от базового урона
            if (dotEffect.Type == TypeOfDots.TYPE_BASE_DMG_PERCENT)
            {
                dotEffect.DotDmg = (int)MathF.Ceiling((float)baseDmg * (float)dotEffect.DotDmg / 100);
            }
        }
    }
}