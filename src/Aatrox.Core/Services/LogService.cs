﻿using System;
using NLog;

namespace Aatrox.Core.Services
{
    public sealed class LogService
    {
        private readonly ILogger _logger;

        public LogService(string name)
        {
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

        public void Log(string level, string message, Exception? e = null)
        {
            switch (level)
            {
                case "Trace":
                    Trace(message);
                    break;
                case "Info":
                case "Information":
                    Info(message);
                    break;
                case "Debug":
                    Debug(message);
                    break;
                case "Error":
                    Error(message, e);
                    break;
                case "Warn":
                case "Warning":
                    Warn(message);
                    break;
                case "Critical":
                    Fatal(message);
                    break;
                default:
                    Info(message);
                    break;
            }
        }

        public void Log(LogLevel level, string message, Exception? e = null)
        {
            switch (level.Name)
            {
                case "Trace":
                    Trace(message);
                    break;
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
                case "Critical":
                    Fatal(message);
                    break;
                default:
                    Info(message);
                    break;
            }
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message, Exception? ex)
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
