using System;

namespace chuni_hands {
    public static class Logger {
        public delegate void LogHandler(string log);

        public static event LogHandler LogAdded;

        public static void Log(LogLevel level, string msg) {
            var time = DateTime.Now.ToString("HH:mm:ss.ff");
            var log = $"{time} [{level}] {msg}";
            LogAdded?.Invoke(log);
        }

        public static void Info(string msg) {
            Log(LogLevel.Info, msg);
        }

        public static void Warning(string msg) {
            Log(LogLevel.Warning, msg);
        }

        public static void Error(string msg) {
            Log(LogLevel.Error, msg);
        }

        public enum LogLevel {
            Info,
            Warning,
            Error
        }
    }
}
