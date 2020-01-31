using UnityEngine;

public class UnityDebugLogger : Core.Service.ILogger
{
    public void Error(object message)
    {
        Debug.LogError(message);
    }

    public void Log(object message)
    {
        Debug.Log(message);
    }

    public void Warning(object message)
    {
        Debug.LogWarning(message);
    }
}
