using core.Logging;


namespace Server.Core.Utility.Logging
{
    public class LokiLogger: ILogger
    {
        private readonly Stash _stash;

        public LokiLogger()
        {
            _stash = new Stash();
        }
        public void Log(LogLevel level, object message)
        {
            _stash.StashLog(level, message);
        }
    }
}