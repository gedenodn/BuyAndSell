namespace BuyAndSell.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private static readonly object _lock = new object();

        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (_lock)
            {
                var logMessage = $"[{DateTime.UtcNow}] [{logLevel.ToString()}] {formatter(state, exception)}{Environment.NewLine}";

                File.AppendAllText(_logFilePath, logMessage);
            }
        }
    }
}

