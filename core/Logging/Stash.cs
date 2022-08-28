using System.Net;
using System.Timers;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace core.Logging
{
    public class Stash
    {
        private class LogStream
        {
            public Dictionary<string, string> stream;
            public string[][] values;
        }
        
        [Serializable]
        private class LogUnit
        {
            public LogStream[] streams;
        }

        private class StashUnit
        {
            public long EventTime { get; }
            public string Message { get; }
            public string Level { get; }

            public StashUnit(LogLevel level, object message)
            {
                Level = level.ToString();
                Message = message as string ?? JsonConvert.SerializeObject(message);
                EventTime = DateTimeOffset.Now.ToUnixTimeSeconds() * 1000000000;
            }

            public string GetLokiLog()
            {
                var dict = new Dictionary<string, string>
                {
                    { "level", Level },
                    { "source", "RT" },
                };

                var streams = new LogUnit
                {
                    streams = new LogStream[1]
                };
                streams.streams[0] = new LogStream
                {
                    stream = dict,
                    values = new []
                    {
                        new []
                        {
                            $"{DateTimeOffset.Now.ToUnixTimeSeconds()*1000000000}", Message
                        }
                    }
                };

                return JsonConvert.SerializeObject(streams);
            }
        }

        private const string Url = "http://logs.bapanda.com:3100/loki/api/v1/push";
        private readonly object _locker = new();
        private readonly List<StashUnit> _units = new();
        private readonly Timer _timer = new(100);

        public Stash()
        {
            _timer.Start();
            _timer.Elapsed += SendLogs;
        }

        private void SendLogs(object sender, ElapsedEventArgs e)
        {
            StashUnit[] units;
            lock (_locker)
            {
                units = _units.ToArray();
                _units.Clear();
            }

            foreach (var unit in units)
            {
                Send(unit.GetLokiLog());
            }
        }

        private void Send(string log)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.WriteAsync(log);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        }

        public void StashLog(LogLevel level, object message)
        {
            lock (_locker)
            {
                _units.Add(new StashUnit(level, message));
            }
        }
    }
}