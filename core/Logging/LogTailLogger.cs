using Newtonsoft.Json;

namespace core.Logging;

public class LogTailLogger : ILogger
{
    public void Log(LogLevel level, object message)
    {
        var logger = NLog.LogManager.GetCurrentClassLogger();

        switch (level)
        {
            case LogLevel.Info:
                logger.Info(JsonConvert.SerializeObject(message));
                break;
            case LogLevel.Warn:
                logger.Warn(JsonConvert.SerializeObject(message));
                break;
            case LogLevel.Error:
                logger.Error(JsonConvert.SerializeObject(message));
                break;
        }
    }
}