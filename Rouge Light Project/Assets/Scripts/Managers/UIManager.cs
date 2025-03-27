using log4net;
using TMPro;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // синглтон

    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(UIManager));

    public TMP_Text textCharacters; // Ссылка на компонент TextMeshPro для Характеристик
    public TMP_Text textDots; // Ссылка на компонент TextMeshPro для Дотов
    public TMP_Text textTimer; // Ссылка на компонент TextMeshPro для Timer
    public TMP_Text textActualHP; // Ссылка на компонент TextMeshPro для ActualHP
    public TMP_Text textCountKill; // Ссылка на компонент TextMeshPro для CountKill
    public TMP_Text textMoney; // Ссылка на компонент TextMeshPro для Money
    public TMP_Text textExp; // Ссылка на компонент TextMeshPro для Exp
    public TMP_Text textLvl; // Ссылка на компонент TextMeshPro для Lvl

    int countCharNameCode = -12; // максимальное количество символов названия кода 
    int countCharValue = 6; // максимальное количество символов значения

    public GameObject damageTextPrefab;

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

        damageTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DamageTextPrefab.prefab");

        if (damageTextPrefab == null)
        {
            log.Error("Awake. Префаб DamageTextPrefab не найден по указанному пути.");
        }
    }

    public void printCharacters(int maxHP, int dmg, float? atkSpeed,
            int moveSpeed, int luck, int critChance, int evadeChance, int armor, int debuffResist,
            int Vampire, int hpFromDropRestore, int dropRadius, int bulletFlySpeed, int bulletTimeAlive)
    {

        if (textCharacters == null)
        {
            log.Error("textCharacters не назначен!");
            return;
        }

        var sb = new StringBuilder();

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

        textCharacters.text = sb.ToString();
    }

    public void printDots(List<UsableDotEffect> usableDotsArray)
    {
        log.Debug("Обращение к printDots");

        if (textDots == null)
        {
            log.Error("textDots не назначен!");
            return;
        }

        var sb = new StringBuilder();

        if (usableDotsArray == null || usableDotsArray.Count == 0)
        {
            sb.AppendLine("No active DOT effects");
            textDots.text = sb.ToString();
            return;
        }

        // Шапка таблицы
        sb.AppendFormat("{0," + countCharNameCode + "} │ {1," + countCharValue + "} │ {2," 
            + countCharValue + "} │ {3," + countCharValue + "} │ {4," + countCharValue + "} │ {5," 
            + countCharNameCode + "} │ {6," + countCharNameCode + "}\n",
            "Effect", "Dmg", "Dur", "Dmg+", "Dur+", "Target", "Type");

        // Разделитель
        sb.AppendLine(new string('─', 12 + 5 * 4 + 12 * 2 + 6 * 6)); // 6 разделителей " │ "

        foreach (var dot in usableDotsArray)
        {
            sb.AppendFormat("{0," + countCharNameCode + "} │ {1," + countCharValue + "} │ {2," 
                + countCharValue + ":F1} │ {3," + countCharValue + "} │ {4," + countCharValue + "} │ {5," 
                + countCharNameCode + "} │ {6," + countCharNameCode + "}\n",
                dot.Code,
                dot.DotDmg,
                dot.DotDur,
                dot.DmgUpgCount,
                dot.DurUpgCount,
                dot.AffectedChar,
                dot.Type);
        }

        textDots.text = sb.ToString();
    }

    public void printTimer(float elapsedTime)
    {
        if (textTimer == null)
        {
            log.Error("textTimer не назначен!");
            return;
        }
        float hours = Mathf.FloorToInt(elapsedTime / 360);
        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);
        double miliSeconds = Math.Round((elapsedTime % 1), 5) * 100000;

        textTimer.text = string.Format("{0:00}:{1:00}:{2:00}.{3:00000}", hours, minutes, seconds, miliSeconds);
    }

    public void printActualHP(int actualHP)
    {
        if (textActualHP == null)
        {
            log.Error("textActualHP не назначен!");
            return;
        }

        textActualHP.text = string.Format("ActualHP : {0}", actualHP);
    }

    public void printCountKill(int countKill)
    {
        if (textCountKill == null)
        {
            log.Error("textCountKill не назначен!");
            return;
        }

        textCountKill.text = string.Format("Count kill : {0}", countKill);
    }

    public void printMoney(int money)
    {
        if (textMoney == null)
        {
            log.Error("textMoney не назначен!");
            return;
        }

        textMoney.text = string.Format("Заработаные : {0}", money);
    }

    public void printExp(int currentExp, int maxExp)
    {
        if (textExp == null)
        {
            log.Error("textExp не назначен!");
            return;
        }
        textExp.text = string.Format("Exp : {0} / {1}", currentExp, maxExp);
    }

    public void printLvl(int lvl)
    {
        if (textLvl == null)
        {
            log.Error("textLvl не назначен!");
            return;
        }

        textLvl.text = string.Format("Lvl : {0}", lvl);
    }

    public void printDamage(string damage, Vector3 position)
    {
        if (damageTextPrefab == null)
        {
            log.Error("TakeDamage. Префаб DamageTextPrefab не найден по указанному пути.");
        }

        GameObject damageText = Instantiate(damageTextPrefab, position, Quaternion.identity);
        // Устанавливаем текст
        TextMeshPro textComponent = damageText.GetComponent<TextMeshPro>();
        if (textComponent == null)
        {
            log.Error("TakeDamage. Префаб textComponent не найден.");
        }

        textComponent.text = damage;
        textComponent.color = Color.red;
        textComponent.sortingOrder = 100;

        // Плавно поднимаем текст вверх
        damageText.transform.DOMoveY(transform.position.y + 2f, 2f)
            .SetEase(Ease.OutQuad); // Плавное ускорение и замедление

        // Плавно изменяем прозрачность текста
        textComponent.DOFade(0f, 1.5f)
            .OnComplete(() => Destroy(damageText));
    }
}
