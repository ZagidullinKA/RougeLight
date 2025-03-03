using log4net;
using UnityEngine;
using UnityEngine.SceneManagement;

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



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] // Запуск скрипта после загрузки сцены
    static void OnSceneLoad()
    {
        if (Instance == null) // проверка на существования этого объекта
        {
            log.Info("Сцена загружена, запускаем инициализацию");
            GameObject initializerObject = new GameObject("Observer"); //Создание объекта для рыботы скрипта
            initializerObject.hideFlags = HideFlags.HideInHierarchy; // Скрываем объект в иерархии
            initializerObject.AddComponent<Observer>(); // Добавляем этот скрипт
            DontDestroyOnLoad(initializerObject); // Не уничтожаем объект при загрузке новой сцены
            StartTimer();
            log.Info("Инициализация наблюдателя завершена");
        }
            
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
}
