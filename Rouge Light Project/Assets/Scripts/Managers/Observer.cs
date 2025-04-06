// Импорт необходимых пространств имен
using log4net; // Для системы логирования
using System; // Базовые типы .NET
using System.Collections.Generic; // Для работы с коллекциями (List)
using UnityEngine; // Базовые функции Unity
using UnityEngine.SceneManagement; // Для работы со сценами
using UnityEngine.UIElements; // Для работы с элементами UI

// Класс Observer - центральный наблюдатель за игровым процессом
// Реализует паттерн Singleton для глобального доступа
public class Observer : MonoBehaviour
{
    public static Observer Instance; // синглтон - единственный экземпляр класса

    //Добавляем логирование
    // Инициализация логгера для этого класса
    private static readonly ILog log = LogManager.GetLogger(typeof(Observer));

    // Статические переменные для хранения игровых данных
    private static int moneyAtStart; // Количество денег в начале игры
    private static int raceID; // Идентификатор расы

    // Переменные для работы с игровым временем
    private static float startTime;                     // Время начала отсчета
    private static bool isRunning = false;              // Флаг, указывающий, работает ли таймер
    private static float lastLogTime = 0f;              // Время последнего вывода лога
    private static float logPeriod = 10f;               // Период логирования (в секундах)
    private static float lastOverestimatingTime = 0f;   // Время последнего пересчета сложности
    private static float recalculationPeriod = 60f;     // Период пересчета кривой сложности
    private static float lastGenerationMobsTime = 0f;   // Время последней генерации мобов
    private static float generationMobsPeriod = 10f;    // Период генерации мобов

    // Статистика игрока
    private static int countKill = 0; // Количество убийств
    private static int lvl = 1; // Текущий уровень
    private static int exp = 0; // Текущий опыт
    private static int lvlCount = 0; // Счетчик повышений уровня
    private static List<int> expNextLvl = new List<int>(); // Список опыта для следующих уровней

    // Метод Awake вызывается при инициализации объекта
    private void Awake()
    {
        // Реализация паттерна Singleton
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

        // Инициализация начальных значений
        initializedMoneyAtStart();

        // Обновление UI
        UIManager.Instance.printCountKill(countKill);
        UIManager.Instance.printMoney(0);
        UIManager.Instance.printLvl(lvl);
        ExpSlider.setMaxExp(searchCountLvlUpExp());
    }

    // Метод вызываемый после загрузки сцены
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnSceneLoad()
    {
        if (Instance == null) // проверка на существования этого объекта
        {
            log.Info("Сцена загружена, запускаем инициализацию");

            // Создание объекта Observer если он не существует
            GameObject initializerObject = new GameObject("Observer");
            initializerObject.hideFlags = HideFlags.HideInHierarchy; // Скрываем объект в иерархии
            initializerObject.AddComponent<Observer>(); // Добавляем этот скрипт

            StartTimer();
            log.Info("Инициализация наблюдателя завершена");
        }
    }

    // Инициализация начального количества денег
    private void initializedMoneyAtStart()
    {
        moneyAtStart = MoneyDictionary.GetItemMoneyDictionaryOfCode(MoneyCode.Money).Amount;
    }

    // Метод Update вызывается каждый кадр
    void Update()
    {
        if (isRunning)
        {
            // Расчет прошедшего времени
            float elapsedTime = Time.time - startTime;
            UIManager.Instance.printTimer(elapsedTime);

            // Проверка необходимости вывода лога
            if (Time.time - lastLogTime >= logPeriod)
            {
                log.Debug("Прошло 10 секунд - На данный момент прошло: " + elapsedTime);
                lastLogTime = Time.time;
            }

            // Проверка необходимости пересчета сложности
            if (Time.time - lastOverestimatingTime >= recalculationPeriod)
            {
                // Пересчитываем множители мобов
                GameGeneration.OverestimatingMobsAmount();
                GameGeneration.OverestimatingMobsMultipier();
                lastOverestimatingTime = Time.time;
            }

            // Проверка необходимости генерации мобов
            if (Time.time - lastGenerationMobsTime >= generationMobsPeriod)
            {
                //Генерация мобов
                GameGeneration.GenerationMobs();
                lastGenerationMobsTime = Time.time;
            }
        }
    }

    // Запуск игрового таймера
    public static void StartTimer()
    {
        log.Debug("Таймер запущен");
        startTime = Time.time; // Запоминаем время начала
        isRunning = true; // Запускаем таймер
    }

    // Остановка игрового таймера
    public static void StopTimer()
    {
        isRunning = false; // Останавливаем таймер
    }

    // Увеличение счетчика убийств
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

    // Увеличение опыта
    public static void IncreasetExp(int deathPrice)
    {
        log.Debug("IncreasetExp. - dp = " + deathPrice);
        deathPrice = CheckLvlUp(deathPrice);
        CheckProgressExp(deathPrice);
    }

    // Проверка повышения уровня
    private static int CheckLvlUp(int deathPrice)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Hero playerScript = player.GetComponent<Hero>();

        for (var i = exp + 1; i <= (exp + deathPrice); i++)
        {
            log.Debug("IncreasetExp. Math.Pow(exp, 0.5) % 1 = " + ((float)Math.Pow(exp, 0.5) % 1 == 0) + "  Math.Pow(exp, 0.5) = " + Math.Pow(exp, 0.5) + "  i  = " + i);

            // Проверка условия повышения уровня (когда квадратный корень опыта целое число)
            if ((float)Math.Pow(i, 0.5) % 1 == 0)
            {
                log.Debug("IncreasetExp. Мы вошли в повышение уровня!");
                lvl++; // Повышения уровня героя
                playerScript.IncrementLvl(lvl); // Перерасчет характеристик героя        
                UIManager.Instance.printLvl(lvl);
                lvlCount++; // Увеличиваем счетчик повышений уровня

                deathPrice = deathPrice - (i - exp); // Высчитываем остаток опыта
                exp = i;
                expNextLvl.Add(searchCountLvlUpExp()); // Получаем опыт для следующего уровня

                if (deathPrice != 0)
                    return CheckLvlUp(deathPrice); // Рекурсивная проверка для остатка опыта
            }
        }

        log.Debug("IncreasetExp. expNextLvl = " + expNextLvl + " lvlCount = " + lvlCount + " deathPrice = " + deathPrice);

        // Обновление слайдера опыта
        ExpSlider.AddExp(expNextLvl, lvlCount, deathPrice);
        expNextLvl.Clear();
        lvlCount = 0;
        return deathPrice; // Возвращаем остаток опыта
    }

    // Обновление текущего опыта
    private static void CheckProgressExp(int deathPrice)
    {
        exp += deathPrice;
    }

    // Поиск количества опыта для следующего уровня
    private static int searchCountLvlUpExp()
    {
        log.Debug("searchCountLvlUpExp. IncreasetExp. Ищем новое значение maxExp до след уровня");

        // Поиск ближайшего числа, квадратный корень которого целый
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

    // Увеличение количества денег
    public static void increaseMoney(int countMoney)
    {
        MoneyDictionary.IncreaseAmountItemMoneyDictionaryOfCode(MoneyCode.Money, countMoney);
        UIManager.Instance.printMoney(MoneyDictionary.GetItemMoneyDictionaryOfCode(MoneyCode.Money).Amount - moneyAtStart);
    }
}