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
    //Добавляем логирование
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
    /// Определяет и возвращает цвет стреляющего объекта для применения к пуле
    /// </summary>
    /// <returns>
    /// Возвращает Color структуру, содержащую RGBA-значение цвета:
    /// - Для игрока: цвет с объекта Triangle
    /// - Для врага: цвет первого найденного SpriteRenderer'а
    /// - Белый цвет (Color.white) если цвет определить не удалось
    /// </returns>
    private Color GetShooterColor()
    {
        // Проверяем, является ли текущий объект игроком (имеет тег "Player")
        // Это важно, так как визуальное представление игрока может отличаться от врагов
        if (gameObject.CompareTag("Player"))
        {
            // Ищем конкретный дочерний объект с именем "Triangle"
            // Triangle - это специальный дочерний объект, содержащий визуальное представление игрока
            Transform triangle = transform.Find("Triangle");

            // Если объект Triangle найден в иерархии
            if (triangle != null)
            {
                // Получаем компонент SpriteRenderer с объекта Triangle
                // SpriteRenderer отвечает за визуальное отображение 2D-объекта
                SpriteRenderer sr = triangle.GetComponent<SpriteRenderer>();

                // Если SpriteRenderer существует и настроен
                if (sr != null)
                {
                    // Возвращаем текущий цвет спрайта
                    // Это позволит пулям игрока соответствовать его цветовой схеме
                    return sr.color;
                }
                // Если SpriteRenderer не найден - продолжаем выполнение
            }
            // Если Triangle не найден - продолжаем выполнение
        }
        // Проверяем, является ли текущий объект врагом (имеет тег "Enemy")
        else if (gameObject.CompareTag("Enemy"))
        {
            // Для врагов используем более общий подход:
            // Ищем любой SpriteRenderer на текущем объекте или его потомках
            // GetComponentInChildren рекурсивно проверяет всю иерархию объектов
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

            // Если SpriteRenderer найден
            if (sr != null)
            {
                // Возвращаем цвет спрайта врага
                // Это обеспечит визуальное соответствие пуль врага их внешнему виду
                return sr.color;
            }
            // Если SpriteRenderer не найден - продолжаем выполнение
        }

        // Возвращаем белый цвет по умолчанию в случаях:
        // - Объект не является ни игроком, ни врагом
        // - Не удалось найти нужный SpriteRenderer
        // - Не найден объект Triangle (для игрока)
        // Color.white - это полностью непрозрачный белый цвет (RGBA: 1,1,1,1)
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
        // log.Debug("Кулдаун " + coolDown);

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
            // Получаем компонент SpriteRenderer у только что созданной пули
            // GetComponentInChildren ищет компонент как на самом объекте пули, так и на всех его дочерних объектах
            // Это гарантирует, что мы найдем визуальное представление пули, даже если оно находится во вложенной иерархии
            SpriteRenderer bulletRenderer = bullet.GetComponentInChildren<SpriteRenderer>();

            // Проверяем, был ли найден компонент SpriteRenderer
            // Эта проверка критически важна, чтобы избежать NullReferenceException
            if (bulletRenderer != null)
            {
                // Устанавливаем цвет визуального представления пули
                // GetShooterColor() возвращает цвет, соответствующий стреляющему объекту:
                // - Для игрока берется цвет с дочернего объекта "Triangle"
                // - Для врага берется первый найденный SpriteRenderer
                // - Если ничего не найдено, возвращается белый цвет (Color.white)
                bulletRenderer.color = GetShooterColor();

                // Логируем информацию о примененном цвете для отладки:
                // - bulletRenderer.color - фактический установленный цвет
                // - gameObject.tag - тег стреляющего объекта (Player/Enemy)
                // Это помогает отслеживать визуальные эффекты во время разработки
                Debug.Log($"Пуля получила цвет: {bulletRenderer.color} (Стрелял: {gameObject.tag})");

                // Примечание: В финальной сборке Debug.Log следует заменить на log.Debug
                // для интеграции с системой log4net, но оставлено для наглядности в примере
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
            log.Debug("Layer Tag is " + layerTag + "Layer Index is " + LayerIndex);

            // Устанавливаем время следующего выстрела
            nextFireTime = Time.time + coolDown;
            log.Debug("nexyFireTime " + nextFireTime + " Time.time " + Time.time);
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

        // хуяк=хуяк и в коммит
    }

    //Присваивание пуле тега в соответствии с тегом стреляющего
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
            // Логируем параметры эффекта
            log.Debug("dotEffect.DotDmg is " + dotEffect.DotDmg +
                " and dotEffect.DotDur is " + dotEffect.DotDur);

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
            log.Debug("dotEffect.type is " + dotEffect.Type);
            if (dotEffect.Type == TypeOfDots.TYPE_BASE_DMG_PERCENT)
            {
                log.Debug("Множитель " + (float)dotEffect.DotDmg / 100 + " Умноженный урон до округления " + (float)baseDmg * (float)dotEffect.DotDmg / 100 + " Округленный урон " + (int)MathF.Ceiling((float)baseDmg * (float)dotEffect.DotDmg / 100));
                // Пересчитываем урон DOT в процентах от базового урона
                dotEffect.DotDmg = (int)MathF.Ceiling((float)baseDmg * (float)dotEffect.DotDmg / 100);
            }
        }
    }
}