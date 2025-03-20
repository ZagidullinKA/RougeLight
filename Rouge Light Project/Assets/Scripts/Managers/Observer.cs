using log4net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Observer : MonoBehaviour
{
    public static Observer Instance; // синглтон

    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Observer));

    private static int moneyAtStart;
    private static int raceID;

    private static float startTime; // Время начала отсчета
    private static bool isRunning = false; // Флаг, указывающий, работает ли таймер
    private static float lastLogTime = 0f; // Время последнего вывода лога
    private static float logPeriod = 10f; // Время последнего вывода лога
    private static float lastOverestimatingTime = 0f; // Время последнего вывода лога
    private static float recalculationPeriod = 60f; // Раз в какое время должна пересчитываться кривая сложности
    private static float lastGenerationMobsTime = 0f; // Время последней генерации мобов
    private static float generationMobsPeriod = 5f; // Раз в какое время происходит генерация мобов

    private static int countKill = 0;
    private static int lvl = 1;
    private static int exp = 0;
    private static int lvlCount = 0;
    private static List<int> expNextLvl = new List<int>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожаем объект при загрузке новой сцены
            log.Info("Observer initialized.");
        }
        else
        {
            log.Warn("Duplicate Observer destroyed.");
            Destroy(gameObject);
        }
        initializedMoneyAtStart();
        UIManager.Instance.printCountKill(countKill);
        UIManager.Instance.printMoney(0);
        UIManager.Instance.printLvl(lvl);
        ExpSlider.setMaxExp(searchCountLvlUpExp()); 
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] // Запуск скрипта после загрузки сцены
    static void OnSceneLoad()
    {
        if (Instance == null) // проверка на существования этого объекта
        {
            log.Info("Сцена загружена, запускаем инициализацию");
            GameObject initializerObject = new GameObject("Observer"); //Создание объекта для рыботы скрипта
            initializerObject.hideFlags = HideFlags.HideInHierarchy; // Скрываем объект в иерархии
            initializerObject.AddComponent<Observer>(); // Добавляем этот скрипт
            StartTimer();
            log.Info("Инициализация наблюдателя завершена");
        }
    }

    private void initializedMoneyAtStart()
    {
        moneyAtStart = MoneyDictionary.GetItemMoneyDictionaryOfCode("money").Amount;
    }

    void Update()
    {
        if (isRunning)
        {
            float elapsedTime = Time.time - startTime; // Прошедшее время
            UIManager.Instance.printTimer(elapsedTime);


            if (Time.time - lastLogTime >= logPeriod) // Проверка вывода лога
            {
                log.Debug("Прошло 10 секунд - На данный момент прошло: " + elapsedTime);
                lastLogTime = Time.time; 
            }
            if (Time.time - lastOverestimatingTime >= recalculationPeriod) // Проверка пересчет множителей
            {
                // Пересчитываем множители мобов
                GameGeneration.OverestimatingMobsAmount();
                GameGeneration.OverestimatingMobsMultipier();
                lastOverestimatingTime = Time.time;
            }
            if (Time.time - lastGenerationMobsTime >= generationMobsPeriod) // Проверка генерации мобов
            {
                //Генерация мобов
                GameGeneration.GenerationMobs();
                lastGenerationMobsTime = Time.time;
            }
        }
    }

    public static void StartTimer()
    {
        log.Debug("Таймер запущен");
        startTime = Time.time; // Запоминаем время начала
        isRunning = true; // Запускаем таймер
    }

    public static void StopTimer()
    {
        isRunning = false; // Останавливаем таймер
    }

    public static void IncrementCountKill(int deathPrice)
    {
        if (Instance == null)
        {
            log.Error("Observer is not initialized!");
            return;
        }

        countKill++;
        UIManager.Instance.printCountKill(countKill);

        IncreasetExp(deathPrice);
    }

    public static void IncreasetExp(int deathPrice)
    {
        log.Debug("IncreasetExp. - dp = " + deathPrice);
        deathPrice = CheckLvlUp(deathPrice);
        CheckProgressExp(deathPrice);
    }

    private static int CheckLvlUp(int deathPrice)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Hero playerScript = player.GetComponent<Hero>();

        for (var i = exp + 1; i <= (exp + deathPrice); i++)
        {
            log.Debug("IncreasetExp. Math.Pow(exp, 0.5) % 1 = " + ((float)Math.Pow(exp, 0.5) % 1 == 0) + "  Math.Pow(exp, 0.5) = " + Math.Pow(exp, 0.5) + "  i  = " + i);
            if ((float)Math.Pow(i, 0.5) % 1 == 0)
            {
                log.Debug("IncreasetExp. Мы вошли в повышение уровня!");
                lvl++;
                playerScript.IncrementLvl(lvl);
                UIManager.Instance.printLvl(lvl);
                lvlCount++;
                
                deathPrice = deathPrice - (i - exp);
                exp = i;
                expNextLvl.Add(searchCountLvlUpExp());
                if (deathPrice != 0)
                    return CheckLvlUp(deathPrice);
            }
        }
        log.Debug("IncreasetExp. expNextLvl = " + expNextLvl + " lvlCount = " + lvlCount + " deathPrice = " + deathPrice);
        if (expNextLvl != null || expNextLvl.Count != 0)
        {
            for (var i = 0; i < expNextLvl.Count; i++)
                log.Debug("IncreasetExp. expNextLvl[" + i + "] = " + expNextLvl[i]);
        } else
        {
            log.Debug("IncreasetExp. expNextLvl is null or empty.");
        }


        ExpSlider.AddExp(expNextLvl, lvlCount, deathPrice);
        expNextLvl.Clear();
        lvlCount = 0;
        return deathPrice;
    
    }

    private static void CheckProgressExp(int deathPrice)

    {
        exp += deathPrice;
    }

    private static int searchCountLvlUpExp()
    {
        log.Debug("searchCountLvlUpExp. IncreasetExp. Ищем новое значение maxExp до след уровня");
        for (var i = exp + 1; i > 0; i++)
        {
            if (Math.Pow(i, 0.5) % 1 == 0)
            {
                log.Debug("searchCountLvlUpExp. IncreasetExp. Нашли, след значение " + i);
                return i;
            }
        }
        log.Error("searchCountLvlUpExp. IncreasetExp. Не нашли след значение уровня!!!!!! ");
        return 0;
    }

    public static void increaseMoney(int countMoney)
    {
        MoneyDictionary.increaseAmountItemMoneyDictionaryOfCode("money", countMoney);
        UIManager.Instance.printMoney(MoneyDictionary.GetItemMoneyDictionaryOfCode("money").Amount - moneyAtStart);
    }
}
