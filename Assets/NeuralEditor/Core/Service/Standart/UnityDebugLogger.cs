using UnityEngine;

public class UnityDebugLogger : Core.Service.ILogger
{
    public void Error(object message, object context)
    {
        Object unityContext = context as Object;

        if (unityContext != null)
        {
            Debug.LogError(message, unityContext);
        }
        else
        {
            Debug.LogError(message);
        }
    }

    public void Log(object message, object context)
    {
        Object unityContext = context as Object;

        if (unityContext != null)
        {
            Debug.Log(message, unityContext);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void Warning(object message, object context)
    {
        Object unityContext = context as Object;

        if (unityContext != null)
        {
            Debug.LogWarning(message, unityContext);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }
}
