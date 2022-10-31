using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DR.Logging.Core;

namespace DR.Logging
{
    public class Configuration
    {
        internal static bool s_logToConsole = true;
        internal static LogFile s_logFile { get; set; } = new LogFile();
        internal static int s_maxSize { get; set; } = 0;
        internal static TimeZoneInfo s_timeZoneInfo { get; private set; } = TimeZoneInfo.Local;
        internal static Colors s_consoleColors { get; private set; } = new Colors();

        /// <summary>
        /// Maximum size of a log file (1 GigaByte)
        /// </summary>
        private static int s_absoluteMaxSize = 1000000000;

        public class Colors
        {
            public string Debug { get; set; } = "#005f87";
            public string Trace { get; set; } = "#00875f";
            public string Info { get; set; } = "#00ffff";
            public string Warn { get; set; } = "#ffff00";
            public string Error { get; set; } = "#ff0000";
            public string Fatal { get; set; } = "#800000";
        }

        internal class Check
        {
            internal enum Type
            {
                None,
                Hex,
                Rgb,
            }

            internal bool _result { get; set; }
            internal Type _type { get; set; }
            internal string _color { get; set; } = string.Empty;
        }

        internal class LogFile
        {
            internal string s_full { get; set; } = string.Empty;
            internal string s_directory { get; set; } = string.Empty;
            internal string s_filename { get; set; } = string.Empty;
            internal string s_extension { get; set; } = string.Empty;
        }

        /// <summary>
        /// Configure your settings for DR.Logging.
        /// </summary>
        /// <param name="logToConsole">Indicate if library should output to console or not.</param>
        /// <param name="logPath">Path of the file that you want to log to.</param>
        /// <param name="maxSize">Maximum size of the log file (in bytes). This cannot be more than 1GB.</param>
        /// <param name="timeZone">What timezone you want the application to use.</param>
        /// <param name="colors">What colors you want to show for each different log level.</param>
        /// <exception cref="Exception">Thrown when you pass invalid (HEX/RGB) colors.</exception>
        public Configuration(bool? logToConsole, string? logPath, int? maxSize, TimeZoneInfo? timeZone, Colors? colors)
        {
            if (logToConsole != null)
                s_logToConsole = (bool)logToConsole;
            if (logPath != null)
            {
                string dirName = Path.GetDirectoryName(logPath);
                if (Directory.Exists(dirName))
                    s_logFile = new LogFile()
                    {
                        s_full = logPath,
                        s_directory = dirName,
                        s_filename = Path.GetFileNameWithoutExtension(logPath),
                        s_extension = Path.GetExtension(logPath)
                    };
                else
                    throw new Errors.InvalidPathException("The path you provided does not exist!");

                if (maxSize == null)
                    maxSize = s_absoluteMaxSize;
                else
                {
                    if (maxSize > s_absoluteMaxSize)
                        maxSize = s_absoluteMaxSize;
                }
                s_maxSize = (int)maxSize;
            }
            if (timeZone != null)
                s_timeZoneInfo = timeZone;
            if (colors != null)
            {
                foreach (PropertyInfo? property in colors.GetType().GetProperties())
                {
                    if (property != null)
                    {
                        string color = property.GetValue(colors, null).ToString();
                        Check colorCheck = CheckIfColor(color);
                        if (colorCheck._type == Check.Type.Rgb)
                            property.SetValue(colors, colorCheck._color);
                        else if (!colorCheck._result)
                            throw new Errors.InvalidColorException($"{color} is not a valid hex or rgb color!");
                    }
                }
                s_consoleColors = colors;
            }
        }

        /// <summary>
        /// Check if string contains a valid RGB or HEX value.
        /// </summary>
        /// <param name="text">String containing RGB or HEX value.</param>
        /// <returns>Data about the result.</returns>
        internal static Check CheckIfColor(string text)
        {
            bool hex = IfValidHex(text);
            bool rgb = false;
            int r = 0;
            int g = 0;
            int b = 0;

            List<int> rgbNumbers = text.SplitNumbersInString();
            if (!hex && rgbNumbers.Count == 3)
            {
                r = rgbNumbers[0];
                g = rgbNumbers[1];
                b = rgbNumbers[2];
                rgb = IfValidRgb(r, g, b);
            }

            if (hex)
                return new Check() { _result = true, _type = Check.Type.Hex };
            else if (rgb)
                return new Check() { _result = true, _type = Check.Type.Rgb, _color = $"rgb({r},{g},{b})" };
            else
                return new Check() { _result = false };
        }

        /// <summary>
        /// Check if hex value is valid.
        /// </summary>
        /// <param name="text">String containing a HEX value.</param>
        /// <returns></returns>
        internal static bool IfValidHex(string text) => Regex.IsMatch(text, @"[#][0-9A-Fa-f]{6}\b");

        /// <summary>
        /// Check if provided values together represent a valid RGB value.
        /// </summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <returns></returns>
        public static bool IfValidRgb(int r, int g, int b)
        {
            if (r < 0 || r > 255)
                return false;
            else if (g < 0 || g > 255)
                return false;
            else if (b < 0 || b > 255)
                return false;
            else
                return true;
        }
    }
}
