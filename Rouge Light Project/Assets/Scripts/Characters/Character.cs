using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class Character : MonoBehaviour, IDamageable, IHealable, IAttacker
{
    // Логгер для отладки
    private static readonly ILog log = LogManager.GetLogger(typeof(Character));
    // Ссылка на базовые характеристики персонажа
    [SerializeField] public BaseStats stats;

    protected float lastHandlingAppliedDoTEffectsTime = 0;
    protected float handlingAppliedDoTEffectsPeriod = 1f;
    protected float lastShootTime;
    protected Shooting shooting;
    protected GameObject firePoint;
    private List<GameObject> firePoints = new List<GameObject>(); // список точек для стрельбы FirePoint

    // Инициализация
    protected virtual void Awake()
    {
        if (stats != null)
        {
            stats.InitializeFromDictionary(new DictionaryCharacters());
        }

        stats.ActualHP = stats.MaxHP; // Устанавливаем текущее здоровье на максимальное при старте

        shooting = gameObject.GetComponent<Shooting>();
        InitializeShootingModifier();

        lastShootTime = Time.time;
    }

    public void Shoot()
    {
        // Вычисляем интервал между выстрелами в зависимости от скорости атаки (stats.AtkSpeed).
        // Например, если AtkSpeed = 25, то shootInterval = 1 / 25 = 0.04 секунды (25 выстрелов в секунду).
        float shootInterval = 1f / (float)stats.AtkSpeed;
        if (Time.time >= lastShootTime + shootInterval)             // проверяем, можем ли стрелять (прошло ли время с последнего выстрела)
        {
            lastShootTime = Time.time;                          // обновляем время последнего выстрела
            
            // Применяем модификаторы стрельбы
            ApplyShootingModifiers();
        }
    }

    /// <summary>
    /// Применяет модификаторы стрельбы - обрабатывает точки стрельбы и выполняет выстрелы
    /// </summary>
    protected virtual void ApplyShootingModifiers()
    {
        // Получаем модификаторы второго слота (всегда должен быть хотя бы один)
        var shootingModifiers = stats.GetShootingModifierSecondArray();
        
        // Обрабатываем каждый модификатор
        foreach (var modifierCode in shootingModifiers)
        {
            ProcessShootingModifier(modifierCode);
        }
    }

    /// <summary>
    /// Универсальный метод для обработки модификаторов стрельбы 2 уровня
    /// </summary>
    /// <param name="modifierCode">Код модификатора</param>
    protected virtual void ProcessShootingModifier(TypeOfShootingModifier modifierCode)
    {
        // Получаем данные модификатора из словаря (всегда должен существовать)
        var modifierData = ShootingModifiersDictionary.GetItemShootingModifierOfCode(modifierCode);

        // Универсальная обработка на основе количества выстрелов
        ProcessUniversalShooting(modifierData);
    }

    /// <summary>
    /// Универсальный метод для обработки стрельбы на основе количества выстрелов
    /// </summary>
    /// <param name="modifierData">Данные модификатора</param>
    protected virtual void ProcessUniversalShooting(ItemShootingModifiersDictionary modifierData)
    {
        int shotCount = modifierData.Count;
        
        if (shotCount == 1)
        {
            // Один выстрел - выполняем сразу
            ExecuteShootingFromAllPoints();
        }
        else
        {
            // Несколько выстрелов - запускаем корутину с задержками
            StartCoroutine(ExecuteMultipleShots(shotCount));
        }
    }

    /// <summary>
    /// Выполняет несколько выстрелов с задержками между ними
    /// </summary>
    /// <param name="shotCount">Количество выстрелов</param>
    /// <returns>Корутина</returns>
    protected virtual System.Collections.IEnumerator ExecuteMultipleShots(int shotCount)
    {
        // Вычисляем задержку между выстрелами: 1/(count * atkSpeed)
        float delayBetweenShots = 1f / (shotCount * stats.AtkSpeed);
        
        for (int i = 0; i < shotCount; i++)
        {
            // Выполняем выстрел из всех активных точек
            ExecuteShootingFromAllPoints();
            
            // Если это не последний выстрел, ждем задержку
            if (i < shotCount - 1)
            {
                yield return new WaitForSeconds(delayBetweenShots);
            }
        }
        
        // Обновляем время последнего выстрела для кулдауна
        lastShootTime = Time.time;
    }


    /// <summary>
    /// Выполняет выстрел из всех активных точек стрельбы
    /// </summary>
    protected virtual void ExecuteShootingFromAllPoints()
    {
        // Используем новую систему shotPointsArray
        var shotPoints = stats.GetShotPointsArray();
        if (shotPoints.Count > 0)
        {
            // Стреляем из всех активных точек
            for (int i = 0; i < shotPoints.Count; i++)
            {
                if (shotPoints[i].isActive)
                {
                    // Получаем угол вращения объекта в радианах
                    float objectRotation = transform.eulerAngles.z * Mathf.Deg2Rad;
                    
                    // Получаем повернутое направление стрельбы
                    Vector2 rotatedDirection = shotPoints[i].GetRotatedDirection(objectRotation);
                    
                    // Создаем временную точку стрельбы на основе повернутого направления
                    Vector3 shootPosition = transform.position + (Vector3)rotatedDirection * 0.8f;
                    
                    // Выполняем выстрелы из этой точки (может быть несколько для дробовика)
                    ExecuteShotsFromPoint(shootPosition, rotatedDirection);
                }
            }
        }
    }

    /// <summary>
    /// Выполняет выстрелы из одной точки с учетом модификаторов 3 уровня
    /// </summary>
    /// <param name="shootPosition">Позиция стрельбы</param>
    /// <param name="baseDirection">Базовое направление</param>
    protected virtual void ExecuteShotsFromPoint(Vector3 shootPosition, Vector2 baseDirection)
    {
        // Получаем модификаторы третьего слота
        var thirdLevelModifiers = stats.GetShootingModifierThirdArray();
        
        float spread = 0f;
        int shotsPerPoint = 1;
        
        if (thirdLevelModifiers.Count > 0)
        {
            // Обрабатываем каждый модификатор 3 уровня (всегда должен существовать)
            foreach (var modifierCode in thirdLevelModifiers)
            {
                var modifierData = ShootingModifiersDictionary.GetItemShootingModifierOfCode(modifierCode);
                
                // Вычисляем spread: (1 - accuracy) * 10
                spread = (1f - modifierData.Accuracy) * 10f;
                
                // Количество выстрелов из одной точки (для дробовика)
                shotsPerPoint = modifierData.Count;
                break; // Берем первый модификатор
            }
        }
        
        // Выполняем несколько выстрелов из одной точки (для дробовика)
        for (int i = 0; i < shotsPerPoint; i++)
        {
            shooting.Shot(stats.Dmg, stats.CritChance, stats.BulletFlySpeed, stats.BulletTimeAlive, 
                         stats.GetUsableDots(), shootPosition, baseDirection, spread);
        }
    }

    /// <summary>
    /// Изменяет точки стрельбы в заданном диапазоне углов, равномерно распределяя их и добавляя новые.
    /// Поддерживает диапазоны, пересекающие 0° (например, от 270° до 90°).
    /// </summary>
    /// <param name="a">Начальный угол диапазона (в градусах)</param>
    /// <param name="b">Конечный угол диапазона (в градусах)</param>
    /// <param name="count">Количество новых точек для добавления</param>
    public void RedistributeShotPointsInRange(int a, int b, int count, bool useUpAsZero = false)
    {
        // Получаем текущий массив точек стрельбы
        var shotPointsArray = stats.GetShotPointsArray();

        // 1. Разделение точек по принадлежности к новому диапазону
        List<ShotPoint> pointsInside = new List<ShotPoint>();
        List<ShotPoint> pointsOutside = new List<ShotPoint>();

        foreach (var sp in shotPointsArray)
        {
            bool isInsideRange;
            
            // Проверяем, пересекает ли диапазон 0°
            if (a > b)
            {
                // Диапазон пересекает 0° (например, 270° до 90°)
                // Ищем точки в диапазонах: от a до 360 и от 0 до b
                isInsideRange = (sp.angle >= a && sp.angle <= 360) || (sp.angle >= 0 && sp.angle <= b);
            }
            else
            {
                // Обычный диапазон (например, 0° до 90°)
                isInsideRange = sp.angle >= a && sp.angle <= b;
            }

            if (isInsideRange)
            {
                pointsInside.Add(sp);
            }
            else
            {
                pointsOutside.Add(sp);
            }
        }

        // 2. Равномерное распределение точек внутри диапазона
        int M = pointsInside.Count + count;
        
        // Вычисляем общую длину диапазона
        float rangeLength;
        if (a > b)
        {
            // Диапазон пересекает 0°: от a до 360 + от 0 до b
            rangeLength = (360 - a) + b;
        }
        else
        {
            // Обычный диапазон
            rangeLength = b - a;
        }
        
        float countM = M;
        if (rangeLength != 360)
        {
            if (countM < 2) {
                countM = 1;
            } else {
                countM = countM - 1;
            }
        }

        float uniform_step = rangeLength / countM;

        List<ShotPoint> redistributedPoints = new List<ShotPoint>();
        for (int j = 0; j < M; j++)
        {
            float new_angle = a + uniform_step * j;
            
            // Нормализуем угол в диапазон [0, 360)
            while (new_angle >= 360) new_angle -= 360;
            while (new_angle < 0) new_angle += 360;
            
            float angle_rad = new_angle * Mathf.Deg2Rad;
            
            Vector2 direction;
            if (useUpAsZero)
            {
                // Система координат: 0° = вверх (для героя)
                direction = new Vector2(Mathf.Sin(angle_rad), Mathf.Cos(angle_rad)).normalized;
            }
            else
            {
                // Система координат: 0° = вправо (для мобов)
                direction = new Vector2(Mathf.Cos(angle_rad), Mathf.Sin(angle_rad)).normalized;
            }

            ShotPoint newPoint = new ShotPoint();
            newPoint.angle = new_angle;
            newPoint.direction = direction;
            newPoint.isActive = true;
            redistributedPoints.Add(newPoint);
        }

        // 3. Формирование итогового массива точек
        List<ShotPoint> result = new List<ShotPoint>();
        result.AddRange(pointsOutside);
        result.AddRange(redistributedPoints);

        // Сортировка по углу
        result.Sort((sp1, sp2) => sp1.angle.CompareTo(sp2.angle));

        // 4. Подготовка данных для стрельбы
        // (angle, direction, isActive уже заданы выше)

        // Обновляем массив точек стрельбы в stats
        log.Debug("RedistributeShotPointsInRange. uniform_step: " + uniform_step);
        log.Debug("RedistributeShotPointsInRange. rangeLength: " + rangeLength);
        log.Debug("RedistributeShotPointsInRange. Итоговый массив точек:");
        foreach (var sp in result)
        {
            log.Debug($"ShotPoint: angle={sp.angle}");
        }

        stats.SetShotPointsArray(result);
    }


    // Получение значения конкретной характеристики персонажа
    public virtual float? GetStat(string statName)
    {
        if (stats == null)
        {
            log.Debug("Stats не инициализирован!"); // проверяем инициализацию stats
            return null;
        }

        return stats.GetStat(statName);
    }

    public virtual void SetStat(string code, float? value)
    {
        if (stats == null)
        {
            log.Debug("Stats не инициализирован!");
            return;
        }

        // преобразуем строку code в enum statCode
        if (Enum.TryParse<CharacterStatCode>(code, ignoreCase: true, out CharacterStatCode statCode))
        {
            stats.SetStat(code, value); // устанавливаем новое значение характеристики
            ApplySpecialEffects(code);  // на случай дополнительных побочных эффектов, если нужно
        }
    }

    protected virtual void ApplySpecialEffects(string statName)
    {
        log.Debug("Метод не переопределен");
    }

    protected virtual void TakeDamage(int damage, TypeOfDamage typeDamage)
    {

        stats.ActualHP -= damage;

        if (stats.IsEnemy)
        {
            if (typeDamage == TypeOfDamage.TYPE_ATTACK)
                UIManager.Instance.printDamage(damage.ToString(), transform.position);
        }

        log.Debug($"Получен урон от атаки: {damage}. Текущее здоровье: {stats.ActualHP}");
        if (stats.ActualHP <= 0)
        {
            Die();
        }
    }

    public virtual void CalculateDamageAfterArmor(int damage, TypeOfDamage typeDamage)
    {
        int damageAfterArmor = damage - stats.Armor;
        if (damageAfterArmor < 0) damageAfterArmor = 0;
        TakeDamage(damageAfterArmor, typeDamage);
    }

    // Метод проверки уклонения от атаки
    public bool TryDodge()
    {
        float randomValue = UnityEngine.Random.value; // генерируем случайное число от 0 до 1
        float evadeChanceMoment = 1 / stats.EvadeChance;
        log.Debug(randomValue);
        log.Debug(randomValue < evadeChanceMoment);
        return randomValue > evadeChanceMoment;
    }

    public void TakeDots(List<UsableDotEffect> forcedDotsArray)
    {
        if (forcedDotsArray.Count == 0) { return; }
        log.Debug("HandlingAppliedDoTEffects. Начало обработки DoT эффектов от атаки");

        foreach (var itemForcedDots in forcedDotsArray)
        {
            bool checkAvailability = true;
            foreach (var itemRecievedDot in stats.GetRecievedDots())
            {
                if (itemForcedDots.Code == itemRecievedDot.Code)
                {
                    itemRecievedDot.DotDur = itemForcedDots.DotDur;
                    itemRecievedDot.DotDmg += 1;
                    itemRecievedDot.Count += 1;
                    itemRecievedDot.Tick = 0;
                    checkAvailability = false;
                    break;
                }
            }
            if (checkAvailability)
            {
                log.Debug("HandlingAppliedDoTEffects. Добавляем новый itemForcedDots.Code = " + itemForcedDots.Code);
                log.Debug("HandlingAppliedDoTEffects. itemForcedDots.DotDmg = " + itemForcedDots.DotDmg);
                stats.SetRecievedDots(new RecievedDotEffect(itemForcedDots));
            }
        }
    }

    public class ItemPrintDot
    {
        public string code;
        public int damage;

        public ItemPrintDot(string code, int damage)
        {
            this.code = code;
            this.damage = damage;
        }
    }

    public void HandlingAppliedDoTEffects()
    {
        if (stats.GetRecievedDots().Count <= 0)
        {
            lastHandlingAppliedDoTEffectsTime = Time.time;
            return;
        }

        if (Time.time - lastHandlingAppliedDoTEffectsTime < handlingAppliedDoTEffectsPeriod)
        {
            return;
        }



        log.Debug("HandlingAppliedDoTEffects. Начало обработки DoT");
        lastHandlingAppliedDoTEffectsTime = Time.time;
        List<RecievedDotEffect> removeRecievedDotsArray = new List<RecievedDotEffect>();
        List<ItemPrintDot> takeDamageList = new List<ItemPrintDot>();
        foreach (var itemRecievedDot in stats.GetRecievedDots())
        {
            log.Debug("HandlingAppliedDoTEffects. Обрабатываем itemRecievedDot.Code = " + itemRecievedDot.Code);

            int countedDotDmg = 0;
            switch (itemRecievedDot.Type)
            {
                case TypeOfDots.TYPE_PERCENT:
                    countedDotDmg = (int)Math.Ceiling((float)(stats.MaxHP / 100 * itemRecievedDot.DotDmg));
                    break;
                case TypeOfDots.TYPE_FIXED:
                case TypeOfDots.TYPE_BASE_DMG_PERCENT:
                    countedDotDmg = itemRecievedDot.DotDmg;
                    break;
                default:
                    log.Error("HandlingAppliedDoTEffects. Неизвестный тип DoT не обработан!");
                    break;
            }

            countedDotDmg -= stats.DebuffResist;

            if (countedDotDmg < 0)
            {
                countedDotDmg = 0;
            }


            if (itemRecievedDot.AffectedChar == CharacterStatCode.ActualHP)
            {
                takeDamageList.Add(new ItemPrintDot(itemRecievedDot.Code.ToString(), countedDotDmg));
                if (countedDotDmg != 0)
                    TakeDamage(countedDotDmg, TypeOfDamage.TYPE_DOT);

                if (itemRecievedDot.DotDur <= 1)
                {
                    removeRecievedDotsArray.Add(itemRecievedDot);
                }
                else
                {
                    itemRecievedDot.DotDur--;
                }

                continue;
            }

            float? affectedCharCurrentvalue = GetStat(itemRecievedDot.AffectedChar.ToString());
            if (itemRecievedDot.Tick < 1)
            {
                log.Debug("HandlingAppliedDoTEffects. item.AffectedChar = " + itemRecievedDot.AffectedChar + ", countedDotDmg = " + -countedDotDmg);
                if (affectedCharCurrentvalue == null)
                {
                    log.Error("В характеристиках нет поля item.AffectedChar = " + itemRecievedDot.AffectedChar);
                }
                if (affectedCharCurrentvalue - countedDotDmg < 0
                    || (itemRecievedDot.AffectedChar != CharacterStatCode.DebuffResist
                    && itemRecievedDot.AffectedChar != CharacterStatCode.Armor))
                {
                    countedDotDmg = (int)affectedCharCurrentvalue;
                }

                SetStat(itemRecievedDot.AffectedChar.ToString(), -countedDotDmg);
                itemRecievedDot.AffectedDamage += countedDotDmg;
            }
            itemRecievedDot.Tick++;



            log.Debug("HandlingAppliedDoTEffects.item.DotDur = " + itemRecievedDot.DotDur);
            if (itemRecievedDot.DotDur <= 1)
            {
                log.Debug("HandlingAppliedDoTEffects. Восстанавливаем в исходное состояние");
                SetStat(itemRecievedDot.AffectedChar.ToString(), itemRecievedDot.AffectedDamage);
                removeRecievedDotsArray.Add(itemRecievedDot);
            }
            else
            {
                itemRecievedDot.DotDur--;
            }

        }

        if (takeDamageList.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < takeDamageList.Count; i++)
            {
                var item = takeDamageList[i];
                sb.Append(item.code + " : " + item.damage);

                if (i < takeDamageList.Count - 1)
                {
                    sb.Append(" | ");
                }
            }
            if (stats.IsEnemy)
            {
                UIManager.Instance.printDamage(sb.ToString(), transform.position);
            }

            takeDamageList.Clear();
        }

        stats.RemoveRecievedDots(removeRecievedDotsArray);
        removeRecievedDotsArray.Clear();
    }

    // Реализация IHealable
    public virtual void Heal(int amount)
    {
        stats.ActualHP += amount;
        if (stats.ActualHP > stats.MaxHP)
        {
            stats.ActualHP = stats.MaxHP;
        }
    }

    // Обработка модификаторов атаки в зависимости от уровня
    protected virtual void AddShootingModifier(string modifierCode)
    {
        // Парсим код модификатора
        if (!Enum.TryParse<TypeOfShootingModifier>(modifierCode, out TypeOfShootingModifier modifierType))
        {
            log.Error($"Не удалось распарсить код модификатора: {modifierCode}");
            return;
        }

        // Находим модификатор в справочнике
        var modifierData = ShootingModifiersDictionary.GetItemShootingModifierOfCode(modifierType);
        if (modifierData == null)
        {
            log.Error($"Модификатор не найден в справочнике: {modifierCode}");
            return;
        }

        int lvl = modifierData.Lvl;
        int count = modifierData.Count;
        int[] range = modifierData.Range; // Получаем range из справочника [start, end]

        if (range == null && lvl == 1)
        {
            log.Error($"Обработка модификатора: {modifierCode}, уровень: {lvl}, количество: {count}, диапазон: null");
        }

        

        // Определяем, является ли персонаж врагом
        bool isEnemy = stats.IsEnemy;
        bool useUpAsZero = !isEnemy; // Герой использует useUpAsZero = true, моб = false

        switch (lvl)
        {
            case 1:
                // Для уровня 1 - вызываем метод изменения точек стрельбы
                RedistributeShotPointsInRange(range[0], range[1], count, useUpAsZero);
                log.Debug($"Применен модификатор уровня 1: {modifierCode} с диапазоном [{range[0]}, {range[1]}] (useUpAsZero = {useUpAsZero})");

                break;
                
            case 2:
                // Для уровня 2 - заменяем имеющийся код в shootingModifierSecondArray
                var secondArray = stats.GetShootingModifierSecondArray();
                string oldModifier2 = secondArray.Count > 0 ? secondArray[0].ToString() : "отсутствовал";
                if (secondArray.Count > 0)
                {
                    // Удаляем старый модификатор
                    stats.RemoveShootingModifierSecond(secondArray[0]);
                }
                // Добавляем новый
                stats.AddShootingModifierSecond(modifierType);
                log.Debug($"Заменен модификатор уровня 2: {oldModifier2} → {modifierCode}");
                break;
                
            case 3:
                // Для уровня 3 - заменяем имеющийся код в shootingModifierThirdArray
                var thirdArray = stats.GetShootingModifierThirdArray();
                string oldModifier3 = thirdArray.Count > 0 ? thirdArray[0].ToString() : "отсутствовал";
                if (thirdArray.Count > 0)
                {
                    // Удаляем старый модификатор
                    stats.RemoveShootingModifierThird(thirdArray[0]);
                }
                // Добавляем новый
                stats.AddShootingModifierThird(modifierType);
                log.Debug($"Заменен модификатор уровня 3: {oldModifier3} → {modifierCode}");
                break;
                
            default:
                log.Error($"Неизвестный уровень модификатора: {lvl}");
                break;
        }
    }

    // Инициализация модификаторов стрельбы - генерация базовых модификаторов из справочника
    protected virtual void InitializeShootingModifier()
    {
        // Получаем все модификаторы с типом "base" из справочника
        var allModifiers = ShootingModifiersDictionary.GetItemsShootingModifiersDictionary();
        var baseModifiers = allModifiers.Where(modifier => modifier.Type == "base").ToList();

        log.Debug($"Найдено {baseModifiers.Count} базовых модификаторов для инициализации");

        // Применяем каждый базовый модификатор
        foreach (var modifier in baseModifiers)
        {
            AddShootingModifier(modifier.Code.ToString());
        }
    }

    // Метод для уничтожения персонажа
    protected virtual void Die()
    {
        log.Debug(gameObject.name + " умер.");
        Destroy(gameObject);
    }
}