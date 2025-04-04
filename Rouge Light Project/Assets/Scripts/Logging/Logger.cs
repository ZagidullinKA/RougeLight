// Импорт необходимых пространств имен Unity
using UnityEngine;

// Импорт пространств имен log4net для работы с логированием
using log4net;       // Основные интерфейсы логирования (ILog)
using log4net.Config; // Для конфигурации через XmlConfigurator
using System.IO;     // Для работы с файловой системой (Path, File)

// Класс Logger - компонент Unity для инициализации системы логирования log4net
public class Logger : MonoBehaviour
{
    // Статическое поле для логгера, доступного во всей программе
    // readonly гарантирует, что ссылка не изменится после инициализации
    private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

    // Метод Awake вызывается Unity при создании объекта
    void Awake()
    {
        // Формируем путь к файлу конфигурации log4net в папке Resources
        string configPath = Path.Combine(Application.dataPath, "Resources/log4net.config");

        // Проверяем существование файла конфигурации
        if (File.Exists(configPath))
        {
            // Настраиваем log4net из XML-файла конфигурации
            XmlConfigurator.Configure(new FileInfo(configPath));

            // Записываем информационное сообщение о успешной инициализации
            log.Info("Логгер инициализирован");

            // Сохраняем объект между сценами (существующий комментарий сохранен)
            DontDestroyOnLoad(gameObject); // Сохраняем между сценами
        }
        else
        {
            // Выводим ошибку в консоль Unity, если файл конфигурации не найден
            Debug.LogError("Файл log4net.config не найден!");
        }
    }
}