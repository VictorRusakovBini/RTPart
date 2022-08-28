using Newtonsoft.Json;

namespace core.Logging;

public class ConsoleLogger: ILogger
{
    public void Log(LogLevel level, object message)
    {
        Console.WriteLine($"{level}:{JsonConvert.SerializeObject(message)}");
    }
}