# DR.Logging
A small logging library. 

## Usage
```cs
using DR.Logging;

namespace DRL_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Write logs with the default settings
            Log.Debug("Debug");
            Log.Trace("Trace");
            Log.Info("Info");
            Log.Warn("Warning");
            Log.Error("Error");
            Log.Fatal("Fatal");

            // Specify if the library logs to the console
            bool logToConsole = true;

            // Make the library output log results to a .json file
            string logPath = $"{Assembly.GetEntryAssembly().Location}/logging.json";

            // Set the maximum size of a log file (example has a max size of 1 MB)
            int maxSize = 1000000;

            // Set what timezone the library uses
            TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time")

            // Set what colors the library uses for different log levels (use either RGB or HEX).
            // NOTE: not all consoles/terminals support all colors.
            // This library uses https://spectreconsole.net/ to show colors in console.
            Configuration.Colors colors = new()
            {
                Debug = "0, 175, 255",
                Trace = "#00d700",
                Info = "69, 174, 206",
                Warn = "#5f00ff",
                Error = "95,135,135",
                Fatal = "#5f5f00",
            };

            // Customize what settings the library uses.
            // NOTE: you only need to set this once. These settings will be applied globally
            _ = new Configuration(logToConsole, logPath, maxSize, timezone, colors);

            // Log with new settings applied
            Log.Debug("Debug");
            Log.Trace("Trace");
            Log.Info("Info");
            Log.Warn("Warning");
            Log.Error("Error");
            Log.Fatal("Fatal");
        }
    }
}
```

This library uses [Spectre.Console](https://spectreconsole.net/) to show colors in console.
If the colors you specified don't show make sure the console/terminal you're using actually support them.