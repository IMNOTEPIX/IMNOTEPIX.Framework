using System;
using System.IO;

namespace IMNOTEPIX.Framework.fCore
{
    /// <summary>
    /// Lightweight file logger for the IMNOTEPIX framework.
    /// Writes timestamped entries to scripts/IMNOTEPIX/IMNOTEPIX.log.
    /// Thread-safe. Resets the log file each session on first write.
    /// </summary>
    public static class Logger
    {
        // ----------------------------------------
        // Configuration
        // ----------------------------------------

        private static readonly string LogDirectory = Path.Combine("scripts", "IMNOTEPIX");
        private static readonly string LogPath = Path.Combine(LogDirectory, "IMNOTEPIX.log");
        private static readonly object _lock = new object();
        private static bool _initialized = false;

        // ----------------------------------------
        // Log Levels
        // ----------------------------------------

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Debug
        }

        // ----------------------------------------
        // Public API
        // ----------------------------------------

        /// <summary>Logs an informational message.</summary>
        public static void Info(string message)
            => Write(LogLevel.Info, null, message);

        /// <summary>Logs an informational message under a named category/system.</summary>
        public static void Info(string category, string message)
            => Write(LogLevel.Info, category, message);

        /// <summary>Logs a warning.</summary>
        public static void Warning(string message)
            => Write(LogLevel.Warning, null, message);

        /// <summary>Logs a warning under a named category/system.</summary>
        public static void Warning(string category, string message)
            => Write(LogLevel.Warning, category, message);

        /// <summary>Logs an error.</summary>
        public static void Error(string message)
            => Write(LogLevel.Error, null, message);

        /// <summary>Logs an error under a named category/system.</summary>
        public static void Error(string category, string message)
            => Write(LogLevel.Error, category, message);

        /// <summary>Logs an error with an exception.</summary>
        public static void Error(string category, string message, Exception ex)
            => Write(LogLevel.Error, category, $"{message} | {ex.GetType().Name}: {ex.Message}");

        /// <summary>Logs a debug message. Only written when debugEnabled is true.</summary>
        public static void Debug(string message, bool debugEnabled = false)
        {
            if (debugEnabled) Write(LogLevel.Debug, null, message);
        }

        /// <summary>Logs a debug message under a named category/system.</summary>
        public static void Debug(string category, string message, bool debugEnabled = false)
        {
            if (debugEnabled) Write(LogLevel.Debug, category, message);
        }

        /// <summary>
        /// Writes a blank separator line to the log.
        /// Useful for visually grouping related entries.
        /// </summary>
        public static void Separator()
        {
            lock (_lock)
            {
                try
                {
                    EnsureInitialized();
                    File.AppendAllText(LogPath, Environment.NewLine);
                }
                catch { }
            }
        }

        // ----------------------------------------
        // Internal
        // ----------------------------------------

        private static void Write(LogLevel level, string category, string message)
        {
            lock (_lock)
            {
                try
                {
                    EnsureInitialized();

                    string tag = category != null ? $"[{category}] " : string.Empty;
                    string line = $"[{DateTime.Now:HH:mm:ss}] [{LevelLabel(level)}] {tag}{message}";

                    File.AppendAllText(LogPath, line + Environment.NewLine);
                }
                catch { }
            }
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;

            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            // Wipe the log at the start of each session
            if (File.Exists(LogPath))
                File.Delete(LogPath);

            File.AppendAllText(LogPath,
                $"=== IMNOTEPIX Framework | Session started {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}{Environment.NewLine}");

            _initialized = true;
        }

        private static string LevelLabel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info: return "INFO";
                case LogLevel.Warning: return "WARN";
                case LogLevel.Error: return "ERROR";
                case LogLevel.Debug: return "DEBUG";
                default: return "?????";
            }
        }
    }
}