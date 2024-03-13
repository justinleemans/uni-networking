using System;
using System.Collections.Generic;
using System.Globalization;

namespace JeeLee.UniNetworking.Logging
{
    /// <summary>
    /// Provides logging functionality for the networking package
    /// </summary>
    public static class NetworkLogger
    {
        private static readonly Dictionary<LogLevel, Action<string>> LogMethods =
            new Dictionary<LogLevel, Action<string>>();

        /// <summary>
        /// Indicates whether logging is enabled.
        /// </summary>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Sets a logging method for a specific log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="method">The logging method.</param>
        public static void SetLogMethod(LogLevel level, Action<string> method)
        {
            LogMethods.TryAdd(level, method);
        }

        /// <summary>
        /// Sets logging methods for multiple log levels.
        /// </summary>
        /// <param name="methods">An array of log level and logging method tuples.</param>
        public static void SetLogMethods(params (LogLevel level, Action<string> method)[] methods)
        {
            foreach (var tuple in methods)
            {
                LogMethods.TryAdd(tuple.level, tuple.method);
            }
        }

        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The log level.</param>
        internal static void Log(object message, LogLevel level = default)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!LogMethods.TryGetValue(level, out var method))
            {
                return;
            }

            method(GetString(message));
        }

        private static string GetString(object message)
        {
            if (message == null)
            {
                return "Null";
            }

            return message is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : message.ToString();
        }
    }
}