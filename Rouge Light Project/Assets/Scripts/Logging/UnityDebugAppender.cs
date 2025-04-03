// Импорт необходимых пространств имен
using log4net.Appender;  // Базовый класс AppenderSkeleton
using log4net.Core;      // Классы для работы с событиями логирования (LoggingEvent)
using UnityEngine;       // Доступ к Unity API (Debug.Log и другие)

// Класс UnityDebugAppender наследуется от AppenderSkeleton из log4net
// и предназначен для перенаправления логов из log4net в систему отладки Unity
public class UnityDebugAppender : AppenderSkeleton
{
    // Переопределенный метод Append, который вызывается для каждого события логирования
    protected override void Append(LoggingEvent loggingEvent)
    {
        // Преобразование события логирования в строку сообщения
        // с использованием настроек форматирования из log4net
        string message = RenderLoggingEvent(loggingEvent);

        // Определение уровня логирования и перенаправление сообщения
        // в соответствующую функцию отладки Unity
        switch (loggingEvent.Level.Name)
        {
            // Для уровней DEBUG и INFO используется обычный Debug.Log
            case "DEBUG":
            case "INFO":
                Debug.Log(message);
                break;

            // Для уровня WARN используется Debug.LogWarning
            case "WARN":
                Debug.LogWarning(message);
                break;

            // Для уровней ERROR и FATAL используется Debug.LogError
            case "ERROR":
            case "FATAL":
                Debug.LogError(message);
                break;
        }
    }
}