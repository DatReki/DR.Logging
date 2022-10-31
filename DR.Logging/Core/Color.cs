using static DR.Logging.Log;

namespace DR.Logging.Core
{
    internal class Color
    {
        /// <summary>
        /// Get the colors used for different log levels
        /// </summary>
        /// <param name="level">The log level</param>
        /// <returns>A string containing a RGB or HEX value</returns>
        internal static string Get(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Configuration.s_consoleColors.Debug;
                case LogLevel.Trace:
                    return Configuration.s_consoleColors.Trace;
                case LogLevel.Info:
                default:
                    return Configuration.s_consoleColors.Info;
                case LogLevel.Warn:
                    return Configuration.s_consoleColors.Warn;
                case LogLevel.Error:
                    return Configuration.s_consoleColors.Error;
                case LogLevel.Fatal:
                    return Configuration.s_consoleColors.Fatal;
            }
        }
    }
}
