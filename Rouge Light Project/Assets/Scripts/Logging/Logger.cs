using UnityEngine;
using log4net;
using log4net.Config;
using System.IO;

public class Logger : MonoBehaviour
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

    void Awake()
    {
        string configPath = Path.Combine(Application.dataPath, "Resources/log4net.config");
        if (File.Exists(configPath))
        {
            XmlConfigurator.Configure(new FileInfo(configPath));
            log.Info("Логгер инициализирован");
        }
        else
        {
            Debug.LogError("Файл log4net.config не найден!");
        }

        // Примеры логирования
        log.Debug("Это сообщение уровня Debug");
        log.Info("Игра запущена");
        log.Warn("Предупреждение: низкий FPS");
        log.Error("Критическая ошибка!");
    }
}