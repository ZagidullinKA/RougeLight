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
            DontDestroyOnLoad(gameObject); // Сохраняем между сценами
        }
        else
        {
            Debug.LogError("Файл log4net.config не найден!");
        }
    }
}