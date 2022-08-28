namespace core.Logging;

public class LogMessage
{
    public LogLevel LogLevel { get; set; }
    public string Message { get; set; }
    public DateTime Time { get; set; }
    public string Source { get; set; }

    public LogMessage(LogLevel level, string message)
    {
        LogLevel = level;
        Message = message;
        Time = DateTime.Now;
        Source = "RT";
    }
}