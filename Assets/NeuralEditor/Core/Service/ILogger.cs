namespace Core.Service
{
    public interface ILogger
    {
        void Log(object message, object context);
        void Warning(object message, object context);
        void Error(object message, object context);
    }
}