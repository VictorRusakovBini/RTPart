namespace core.Logging;

public class Debug
{
    private static ILogger _logger;

    public static void Initialize<T>() where T : ILogger, new()
    {
        _logger = new T();
    }

    public static void Log(object message)
    {
        _logger.Log(LogLevel.Info, message);
    }
    
    public static void Warning(object message)
    {
        _logger.Log(LogLevel.Warn, message);
    }
    
    public static void Error(object message)
    {
        _logger.Log(LogLevel.Error, message);
    }
    
    public static void Exception(Exception message)
    {
        _logger.Log(LogLevel.Error, message);
    }
}