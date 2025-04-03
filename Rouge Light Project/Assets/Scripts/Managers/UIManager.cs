// Импорт необходимых пространств имен
using log4net;       // Для системы логирования
using TMPro;         // Для работы с TextMeshPro
using UnityEngine;   // Базовые функции Unity
using System;        // Базовые типы .NET
using System.Text;   // Для работы с StringBuilder
using System.Collections.Generic; // Для работы с коллекциями
using DG.Tweening;   // Для анимаций
using UnityEditor;   // Для работы с редактором Unity (AssetDatabase)

// Класс UIManager - централизованный менеджер пользовательского интерфейса
// Реализует паттерн Singleton для глобального доступа
public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // синглтон - единственный экземпляр класса

    //Добавляем логирование
    // Инициализация логгера для этого класса
    private static readonly ILog log = LogManager.GetLogger(typeof(UIManager));

    // Ссылки на текстовые элементы UI
    public TMP_Text textCharacters;     // Ссылка на компонент TextMeshPro для Характеристик
    public TMP_Text textUsableDots;     // Ссылка на компонент TextMeshPro для используемых Дотов
    public TMP_Text textRecievedDots;   // Ссылка на компонент TextMeshPro для наложенных Дотов
    public TMP_Text textTimer;          // Ссылка на компонент TextMeshPro для Timer
    public TMP_Text textActualHP;       // Ссылка на компонент TextMeshPro для ActualHP
    public TMP_Text textCountKill;      // Ссылка на компонент TextMeshPro для CountKill
    public TMP_Text textMoney;          // Ссылка на компонент TextMeshPro для Money
    public TMP_Text textExp;            // Ссылка на компонент TextMeshPro для Exp
    public TMP_Text textLvl;            // Ссылка на компонент TextMeshPro для Lvl

    // Настройки форматирования текста
    int countCharNameCode = -12; // максимальное количество символов названия кода (отрицательное значение для выравнивания по левому краю)
    int countCharValue = 6; // максимальное количество символов значения

    // Префаб для отображения урона
    public GameObject damageTextPrefab;

    // Метод Awake вызывается при инициализации объекта
    void Awake()
    {
        // Реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            log.Info("UIManager инициализарован");
            DontDestroyOnLoad(gameObject); // сохранить между сценами
        }
        else
        {
            log.Warn("UIManager Уничтожен");
            Destroy(gameObject);
        }

        // Загрузка префаба для текста урона из Assets
        damageTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DamageTextPrefab.prefab");

        if (damageTextPrefab == null)
        {
            log.Error("Awake. Префаб DamageTextPrefab не найден по указанному пути.");
        }
    }

    // Метод для отображения характеристик персонажа
    public void printCharacters(int maxHP, int dmg, float? atkSpeed,
            int moveSpeed, int luck, int critChance, int evadeChance, int armor, int debuffResist,
            int Vampire, int hpFromDropRestore, int dropRadius, int bulletFlySpeed, int bulletTimeAlive)
    {
        // Проверка наличия ссылки на текстовый компонент
        if (textCharacters == null)
        {
            log.Error("textCharacters не назначен!");
            return;
        }

        // Использование StringBuilder для эффективного построения строки
        var sb = new StringBuilder();

        // Форматированный вывод каждой характеристики с выравниванием
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "maxHP", maxHP);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "dmg", dmg);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + ":F2}\n", "atkSpeed", atkSpeed);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "moveSpeed", moveSpeed);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "luck", luck);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}%\n", "crit", critChance);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}%\n", "evade", evadeChance);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "armor", armor);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "debuffR", debuffResist);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "Vampire", Vampire);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "hpDropR", hpFromDropRestore);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "dropRad", dropRadius);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}\n", "bFlySpeed", bulletFlySpeed);
        sb.AppendFormat("{0," + countCharNameCode + "} = {1," + countCharValue + "}", "bTimeAlive", bulletTimeAlive);

        // Установка сформированного текста
        textCharacters.text = sb.ToString();
    }

    // Метод для отображения активных DOT-эффектов (накладываемых на врагов)
    public void printUsableDots(List<UsableDotEffect> usableDotsArray)
    {
        log.Debug("Обращение к printUsableDots");

        // Проверка наличия текстового компонента
        if (textUsableDots == null)
        {
            log.Error("textDots не назначен!");
            return;
        }

        var sb = new StringBuilder();

        // Проверка на пустой список эффектов
        if (usableDotsArray == null || usableDotsArray.Count == 0)
        {
            sb.AppendLine("No active DOT effects");
            textUsableDots.text = sb.ToString();
            return;
        }

        // Шапка таблицы
        sb.AppendFormat("{0," + countCharNameCode + "} │ {1," + countCharValue + "} │ {2,"
            + countCharValue + "} │ {3," + countCharValue + "} │ {4," + countCharNameCode + "}\n",
            "Effect", "Dmg", "Dur", "Target", "Type");

        // Разделитель
        sb.AppendLine(new string('─', 12 + 5 * 4 + 12 * 2 + 6 * 6)); // 6 разделителей " │ "

        // Добавление информации о каждом DOT-эффекте
        foreach (var dot in usableDotsArray)
        {
            sb.AppendFormat("{0," + countCharNameCode + "} │ {1," + countCharValue + "} │ {2,"
                + countCharValue + ":F1} │ {3," + countCharValue + "} │ {4," + countCharNameCode + "}\n",
                dot.Code,
                dot.DotDmg,
                dot.DotDur,
                dot.AffectedChar,
                dot.Type);
        }

        textUsableDots.text = sb.ToString();
    }

    // Метод для отображения полученных DOT-эффектов (действующих на игрока)
    public void printRecievedDots(List<RecievedDotEffect> recievedDotsArray)
    {
        if (textRecievedDots == null)
        {
            log.Error("textDots не назначен!");
            return;
        }

        var sb = new StringBuilder();

        if (recievedDotsArray == null || recievedDotsArray.Count == 0)
        {
            sb.AppendLine("No active DOT effects");
            textRecievedDots.text = sb.ToString();
            return;
        }

        // Шапка таблицы
        sb.AppendFormat("{0," + countCharNameCode + "} │ {1," + countCharValue + "} │ {2,"
            + countCharValue + "} │ {3," + countCharNameCode + "} │ {4," + countCharNameCode + "}" +
            " │ {5," + countCharValue + "} \n",
            "Effect", "Dmg", "Dur", "affectedDamage", "count", "tick");

        // Разделитель
        sb.AppendLine(new string('─', 12 + 5 * 4 + 12 * 2 + 6 * 6)); // 6 разделителей " │ "

        foreach (var dot in recievedDotsArray)
        {
            sb.AppendFormat("{0," + countCharNameCode + "} │ {1," + countCharValue + "} │ {2,"
                + countCharValue + ":F1} │ {3," + countCharValue + "} │ {4," + countCharNameCode + "}\n",
                dot.Code,
                dot.DotDmg,
                dot.DotDur,
                dot.AffectedChar,
                dot.Count,
                dot.Tick);
        }

        textRecievedDots.text = sb.ToString();
    }

    // Метод для отображения игрового времени
    public void printTimer(float elapsedTime)
    {
        if (textTimer == null)
        {
            log.Error("textTimer не назначен!");
            return;
        }
        // Конвертация времени в часы, минуты, секунды и миллисекунды
        float hours = Mathf.FloorToInt(elapsedTime / 360);
        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);
        double miliSeconds = Math.Round((elapsedTime % 1), 5) * 100000;

        // Форматированный вывод времени
        textTimer.text = string.Format("{0:00}:{1:00}:{2:00}.{3:00000}", hours, minutes, seconds, miliSeconds);
    }

    // Метод для отображения текущего здоровья
    public void printActualHP(int actualHP)
    {
        if (textActualHP == null)
        {
            log.Error("textActualHP не назначен!");
            return;
        }

        textActualHP.text = string.Format("ActualHP : {0}", actualHP);
    }

    // Метод для отображения количества убийств
    public void printCountKill(int countKill)
    {
        if (textCountKill == null)
        {
            log.Error("textCountKill не назначен!");
            return;
        }

        textCountKill.text = string.Format("Count kill : {0}", countKill);
    }

    // Метод для отображения количества денег
    public void printMoney(int money)
    {
        if (textMoney == null)
        {
            log.Error("textMoney не назначен!");
            return;
        }

        textMoney.text = string.Format("Заработаные : {0}", money);
    }

    // Метод для отображения опыта (текущий/максимальный)
    public void printExp(int currentExp, int maxExp)
    {
        if (textExp == null)
        {
            log.Error("textExp не назначен!");
            return;
        }
        textExp.text = string.Format("Exp : {0} / {1}", currentExp, maxExp);
    }

    // Метод для отображения уровня
    public void printLvl(int lvl)
    {
        if (textLvl == null)
        {
            log.Error("textLvl не назначен!");
            return;
        }

        textLvl.text = string.Format("Lvl : {0}", lvl);
    }

    // Метод для отображения урона в виде всплывающего текста
    public void printDamage(string damage, Vector3 position)
    {
        if (damageTextPrefab == null)
        {
            log.Error("TakeDamage. Префаб DamageTextPrefab не найден по указанному пути.");
        }

        // Создание экземпляра префаба с текстом урона
        GameObject damageText = Instantiate(damageTextPrefab, position, Quaternion.identity);

        // Получение компонента TextMeshPro
        TextMeshPro textComponent = damageText.GetComponent<TextMeshPro>();
        if (textComponent == null)
        {
            log.Error("TakeDamage. Префаб textComponent не найден.");
        }

        // Настройка текста
        textComponent.text = damage;
        textComponent.color = Color.red;
        textComponent.sortingOrder = 100; // Установка порядка отрисовки

        // Анимация движения текста вверх
        damageText.transform.DOMoveY(transform.position.y + 2f, 2f)
            .SetEase(Ease.OutQuad); // Плавное ускорение и замедление

        // Анимация исчезновения текста
        textComponent.DOFade(0f, 1.5f)
            .OnComplete(() => Destroy(damageText)); // Уничтожение объекта после завершения анимации
    }
}