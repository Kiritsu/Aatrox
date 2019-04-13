using System;
using System.Reflection;
using NLog;

namespace Aatrox.Core.Logging
{
    public class LogService
    {
        private readonly ILogger _logger;

        public LogService(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Assembly.GetEntryAssembly().FullName;
            }

            _logger = LogManager.GetLogger(name);
        }

        public static LogService GetLogger<T>()
        {
            return new LogService(typeof(T).Name);
        }

        public static LogService GetLogger(string name)
        {
            return new LogService(name);
        }

        public void Log(LogLevel level, string message, Exception e = null)
        {
            switch (level.Name)
            {
                case "Info":
                    Info(message);
                    break;
                case "Debug":
                    Debug(message);
                    break;
                case "Error":
                    Error(message, e);
                    break;
                case "Warn":
                    Warn(message);
                    break;
            }
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message, Exception ex)
        {
            if (ex is null)
            {
                _logger.Error(message);
            }
            else
            {
                _logger.Error(ex, message);
            }
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}
