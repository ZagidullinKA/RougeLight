using log4net.Appender;
using log4net.Core;
using UnityEngine;

public class UnityDebugAppender : AppenderSkeleton
{
    protected override void Append(LoggingEvent loggingEvent)
    {
        string message = RenderLoggingEvent(loggingEvent);

        switch (loggingEvent.Level.Name)
        {
            case "DEBUG":
            case "INFO":
                Debug.Log(message);
                break;
            case "WARN":
                Debug.LogWarning(message);
                break;
            case "ERROR":
            case "FATAL":
                Debug.LogError(message);
                break;
        }
    }
}