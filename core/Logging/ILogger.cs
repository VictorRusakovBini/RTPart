namespace core.Logging;

public interface ILogger
{
    void Log(LogLevel level, object message);
}