namespace Core.Service
{
    public interface ILogger
    {
        void Log(object message);
        void Warning(object message);
        void Error(object message);
    }
}