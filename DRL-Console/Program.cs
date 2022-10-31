using System.Text;
using System.Reflection;
using System.Diagnostics;

using DR.Logging;
using ByteSizeLib;
using Spectre.Console;

namespace DRL_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteTimeZones();
            string logPath = GetExecutablePath("logging.json");

            #region Default configuration
            Console.WriteLine($"Original:");
            Log.Debug("debug test");
            Log.Trace("trace test");
            Log.Info("info test");
            Log.Warn("warning test");
            Log.Error("error test");
            Log.Fatal("fatal test");
            #endregion

            #region Custom configuration
            Console.WriteLine($"\n\n New:");
            Configuration.Colors colors = new()
            {
                Debug = "0, 175, 255",
                Trace = "#00d700",
                Info = "69, 174, 206",
                Warn = "#5f00ff",
                Error = "95,135,135",
                Fatal = "#5f5f00",
            };

            _ = new Configuration(true, logPath, null, TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time"), colors);
            Log.Debug("debug test");
            Log.Trace("trace test");
            Log.Info("info test");
            Log.Warn("warning test");
            Log.Error("error test");
            Log.Fatal("fatal test");
            #endregion

            #region Performance
            int loops = 200;
            int totalLoops = loops * 6;
            Stopwatch sw = new Stopwatch();

            AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn() 
                    { 
                        RemainingStyle = new Style(Color.Grey),
                        IndeterminateStyle = new Style(Color.White),
                        CompletedStyle = new Style(Color.Green)
                    },
                    new PercentageColumn()
                    {
                        Style = new Style(Color.Grey),
                    },
                    new RemainingTimeColumn()
                    {
                        Style = new Style(Color.Grey)
                    },
                    new SpinnerColumn()
                    {
                        Style = new Style(Color.Green)
                    }
                })
                .Start(ctx =>
                {
                    ProgressTask task = ctx.AddTask($"Writing {totalLoops} logs to file");
                    _ = new Configuration(false, logPath, (int)ByteSize.FromKiloBytes(25).Bytes, TimeZoneInfo.Local, null);
                    sw = Stopwatch.StartNew();
                    for (int count = 0; count < loops; count++)
                    {
                        Log.Debug($"debug {count}");
                        Log.Trace($"trace {count}");
                        Log.Info($"info {count}");
                        Log.Warn($"warning {count}");
                        Log.Error($"error {count}");
                        Log.Fatal($"fatal {count}");
                        task.Increment(0.5);
                    }
                });

            Console.WriteLine($"Done writing {totalLoops} logs\n" +
                $"Took: {sw.Elapsed.TotalSeconds} seconds");
            #endregion
        }

        static void WriteTimeZones()
        {
            string path = GetExecutablePath("timezones.txt");
            WriteFileLocation(path);
            File.WriteAllText(path, string.Join(Environment.NewLine, TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id).OrderBy(x => x)));
        }

        static string GetExecutablePath(string? filename = null)
        {
            string file = string.Empty;
            Assembly? executable = Assembly.GetEntryAssembly();
            string? executablePath = string.Empty;
            if (executable != null)
                executablePath = Path.GetDirectoryName(executable.Location);

            if (filename == null && executablePath != null)
                file = executablePath;
            else if (filename != null && executablePath != null)
                file = Path.Combine(executablePath, filename);

            return file;
        }

        private static void WriteFileLocation(string path)
        {
            Console.WriteLine($"Writing {Path.GetFileName(path)} to {Path.GetDirectoryName(path)}");
        }
    }
}