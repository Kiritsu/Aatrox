using NLog;
using System;

namespace Aatrox.Core.Logging
{
    public class Logger
    {
        private readonly ILogger _logger;

        public Logger(string name)
        {
            _logger = LogManager.GetLogger(name);
        }

        public static Logger GetLogger<T>()
        {
            return new Logger(typeof(T).Name);
        }

        public static Logger GetLogger(string name)
        {
            return new Logger(name);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}
