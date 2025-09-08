using log4net;
using System;
using System.Collections.Generic;
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

        CreateFirePoint();
        shooting = gameObject.GetComponent<Shooting>();

        lastShootTime = Time.time;
    }

    protected virtual void CreateFirePoint()
    {
        // Создаем новый GameObject с именем "FirePoint"
        firePoint = new GameObject("FirePoint");

        // Устанавливаем его как дочерний объект персонажа (this.gameObject)
        firePoint.transform.SetParent(this.transform);

        // Настраиваем Transform
        firePoint.transform.localPosition = new Vector3(0f, 0.8f, 0f); // позиция (0, 0.8, 0)
        firePoint.transform.localRotation = Quaternion.identity;       // поворот (0, 0, 0)
        firePoint.transform.localScale = Vector3.one;                  // масштаб (1, 1, 1)

        // Устанавливаем Tag
        firePoint.tag = "PlayerFirePoint";

        // Устанавливаем Layer
        firePoint.layer = LayerMask.NameToLayer("Hero");
    }

    protected virtual void CreateFirePointAround(int count, float radius, float totalAngle)
    {
        // Проверяем корректность входных параметров
        if (count < 1)
        {
            log.Error("Count не может быть меньше 0!");
            return;
        }

        // Ограничиваем totalAngle до 360 градусов (если необходимо)
        if (totalAngle > 360f)
        {
            log.Warn("TotalAngle не может быть больше 360 градусов! Устанавливаем 360.");
            totalAngle = 360f;
        }

        if (totalAngle <= 0f)
        {
            log.Error("TotalAngle не может быть меньше 0!");
            return;
        }

        // Удаляем существующие FirePoint, если они есть (кроме основного)
        foreach (var fp in firePoints)
        {
            if (fp != null && fp != firePoint)
            {
                Destroy(fp);
            }
        }
        firePoints.Clear();

        // Добавляем основной FirePoint в список (если он не в центре координат)
        firePoints.Add(firePoint);

        // Вычисляем угол основного FirePoint относительно центра
        Vector3 firstFirePointPos = firePoint.transform.localPosition;
        float startAngle;
        if (firstFirePointPos == Vector3.zero)
        {
            startAngle = 90f; // по умолчанию угол в 90° (вверх), если FirePoint в центре
        }
        else
        {
            startAngle = Mathf.Atan2(firstFirePointPos.y, firstFirePointPos.x) * Mathf.Rad2Deg;
        }

        // Вычисляем начальный и конечный углы для всех точек
        float adjustedStartAngle;
        float adjustedEndAngle;
        if (totalAngle < 360f)
        {
            // Симметрично относительно основного FirePoint
            adjustedStartAngle = startAngle - (totalAngle / 2f);
            adjustedEndAngle = startAngle + (totalAngle / 2f);
        }
        else
        {
            // Для полного круга начинаем с угла, следующего за основным FirePoint
            adjustedStartAngle = startAngle + (totalAngle / count);
            adjustedEndAngle = startAngle + totalAngle;
        }

        // Вычисляем шаг между точками (totalAngle / (количество точек - 1))
        float angleStep = (adjustedEndAngle - adjustedStartAngle) / (count - 1);

        // Создаем count новых FirePoint
        for (int i = 0; i < count; i++)
        {
            // Вычисляем угол для текущей точки
            float t = (float)i / (count - 1); // интерполированное значение от 0 до 1
            float angle = Mathf.Lerp(adjustedStartAngle, adjustedEndAngle, t);

            // Преобразуем угол в радианы
            float angleRad = angle * Mathf.Deg2Rad;

            // Вычисляем координаты по углу (x = cos(угол), y = sin(угол))
            float x = radius * Mathf.Cos(angleRad);
            float y = radius * Mathf.Sin(angleRad);

            // Создаем новый FirePoint
            GameObject newFirePoint = new GameObject($"FirePoint_{i}");
            newFirePoint.transform.SetParent(this.transform);
            newFirePoint.transform.localPosition = new Vector3(x, y, 0f);
            newFirePoint.transform.localRotation = Quaternion.identity;
            newFirePoint.transform.localScale = Vector3.one;
            newFirePoint.tag = "PlayerFirePoint";
            newFirePoint.layer = LayerMask.NameToLayer("Hero");

            // Добавляем в список
            firePoints.Add(newFirePoint);
        }
    }

    public void Shoot()
    {
        // Вычисляем интервал между выстрелами в зависимости от скорости атаки (stats.AtkSpeed).
        // Например, если AtkSpeed = 25, то shootInterval = 1 / 25 = 0.04 секунды (25 выстрелов в секунду).
        float shootInterval = 1f / (float)stats.AtkSpeed;
        if (Time.time >= lastShootTime + shootInterval)             // проверяем, можем ли стрелять (прошло ли время с последнего выстрела)
        {
            lastShootTime = Time.time;                          // обновляем время последнего выстрела
            
            if (firePoints.Count > 0)
            {
                for (int i = 0; i < firePoints.Count; i++) {
                    shooting.Shot(stats.Dmg, stats.CritChance, stats.BulletFlySpeed, stats.BulletTimeAlive, stats.GetUsableDots(), firePoints[i]);
                }
            } else
            {
                shooting.Shot(stats.Dmg, stats.CritChance, stats.BulletFlySpeed, stats.BulletTimeAlive, stats.GetUsableDots(), firePoint);
            }
            
            
        }
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

    // Метод для уничтожения персонажа
    protected virtual void Die()
    {
        log.Debug(gameObject.name + " умер.");
        Destroy(gameObject);
    }
}