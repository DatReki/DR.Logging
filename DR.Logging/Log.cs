using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

using Spectre.Console;
using Newtonsoft.Json.Linq;
using static DR.Logging.Configuration;

namespace DR.Logging
{
    public class Log
    {
        /// <summary>
        /// String containing the entire json log.
        /// </summary>
        internal static string s_json { get; private set; } = string.Empty;

        internal enum LogLevel
        {
            Debug,
            Trace,
            Info,
            Warn,
            Error,
            Fatal
        }

        /// <summary>
        /// Write a debug log.
        /// </summary>
        /// <param name="message">The message/exception you want to log.</param>
        public static void Debug(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int lineNumber = 0) => Base(LogLevel.Debug, message, method, file, lineNumber);

        /// <summary>
        /// Write a trace log.
        /// </summary>
        /// <param name="message">The message/exception you want to log.</param>
        public static void Trace(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int lineNumber = 0) => Base(LogLevel.Trace, message, method, file, lineNumber);

        /// <summary>
        /// Write a information log.
        /// </summary>
        /// <param name="message">The message/exception you want to log.</param>
        public static void Info(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int lineNumber = 0) => Base(LogLevel.Info, message, method, file, lineNumber);

        /// <summary>
        /// Write a warning log.
        /// </summary>
        /// <param name="message">The message/exception you want to log.</param>
        public static void Warn(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int lineNumber = 0) => Base(LogLevel.Warn, message, method, file, lineNumber);

        /// <summary>
        /// Write a error log.
        /// </summary>
        /// <param name="message">The message/exception you want to log.</param>
        public static void Error(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int lineNumber = 0) => Base(LogLevel.Error, message, method, file, lineNumber);

        /// <summary>
        /// Write a fatal log.
        /// </summary>
        /// <param name="message">The message/exception you want to log.</param>
        public static void Fatal(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int lineNumber = 0) => Base(LogLevel.Fatal, message, method, file, lineNumber);

        /// <summary>
        /// Write log to the logging file.
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message/exception you want to log.</param>
        /// <param name="method"></param>
        /// <param name="file"></param>
        /// <param name="lineNumber"></param>
        internal static void Base(LogLevel level,
            string message,
            string method = "",
            string file = "",
            int lineNumber = 0)
        {
            DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, s_timeZoneInfo);

            if (s_logToConsole)
            {
                string color = Core.Color.Get(level);
                Console.Write($"[{date.ToShortDateString()} {date.ToLongTimeString()}] [");
                try
                {
                    // Set hex color
                    AnsiConsole.Write(new Markup($"[{color}]{level}[/]"));
                }
                catch
                {
                    try
                    {
                        // Set rgb color
                        AnsiConsole.Write(new Markup($"[rgb({color})]{level}[/]"));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                Console.Write($"] [{Path.GetFileName(file)}] [{method}] [{lineNumber}] {message}\n");
            }

            Configuration.LogFile logFile = s_logFile;
            string logPath = logFile.s_full;
            if (logPath != string.Empty)
            {
                if (File.Exists(logPath))
                {
                    // If file does exist read from it.
                    if (s_json == string.Empty)
                        s_json = File.ReadAllText(logPath);

                    // If file/string reached max size 
                    if (s_json.Length > s_maxSize)
                    {
                        MoveFile(date, 0, logPath, logFile.s_directory, logFile.s_filename, logFile.s_extension);
                    }
                }
                
                if (!File.Exists(logPath))
                    s_json = Models.Serialize.ToJson<Models.LogFileModel>(new Models.LogFileModel());



                JObject logs = JObject.Parse(s_json);

                JArray? levelArray = logs[level.ToString().ToLower()] as JArray;
                if (levelArray != null)
                {
                    // Insert new item at the top of the array. Makes it easier to read.
                    levelArray.Insert(0, JToken.FromObject(SetValue(date, file, method, lineNumber, message)));
                    logs[level.ToString().ToLower()] = levelArray;
                }

                s_json = logs.ToString();
                // Write logs to file
                File.WriteAllText(logPath, s_json);
            }
        }

        /// <summary>
        /// Create new LogEntryModel to write to logging file.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="file"></param>
        /// <param name="method"></param>
        /// <param name="lineNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static Models.LogEntryModel SetValue(DateTime date, string file, string method, int lineNumber, string message)
        {
            return new Models.LogEntryModel()
            {
                Date = $"{date.ToString()}",
                File = file,
                Method = method,
                LineNumber = lineNumber,
                Message = message
            };
        }

        /// <summary>
        /// Try move old log file if it exceeds the size limit.
        /// </summary>
        /// <param name="date">DateTime of when the log is being written.</param>
        /// <param name="level">Level of the filename (0/1) higher = more complex filename.</param>
        /// <param name="originalPath">Path of the old log file.</param>
        /// <param name="directory">Directory where to move the old logfile to.</param>
        /// <param name="filename">Filename that the old log file should get.</param>
        /// <param name="extension">Extension that the old log file should get.</param>
        private static void MoveFile(DateTime date, int level, string originalPath, string directory, string filename, string extension)
        {
            string filenameAddition = string.Empty;
            string friendlyShortDate = date.ToShortDateString().Replace('/', '_');

            switch (level)
            {
                case 0:
                default:
                    // Try create log file with just date + short time
                    filenameAddition = $"{friendlyShortDate}-{date.ToShortTimeString().Replace(':', '_')}";
                    break;
                case 1:
                    // If that file already exists create one with hours:minutes:seconds:miliseconds format
                    filenameAddition = $"{friendlyShortDate}-{date.ToString("hh:mm:ss.fff").Replace(' ', '-').Replace(':', '_')}";
                    break;
            }

            try
            {
                string newLocation = Path.Combine(directory, $"{filename}-{filenameAddition}{extension}");
                File.Move(originalPath, newLocation);
            }
            catch (Exception e)
            {
                if (e.HResult == -2147024713)
                    MoveFile(date, 1, originalPath, directory, filename, extension);
                else
                    throw e;
            }
        }
    }
}
