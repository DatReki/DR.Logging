using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using DR.Logging;

namespace DRL_Testing
{
    public class Tests
    {
        /// <summary>
        /// Write debug log
        /// </summary>
        [Test]
        public void DebugLog()
        {
            Log.Debug($"test");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Write trace log
        /// </summary>
        [Test]
        public void TraceLog()
        {
            Log.Trace($"test");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Write info log
        /// </summary>
        [Test]
        public void InfoLog()
        {
            Log.Info($"test");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Write warning log
        /// </summary>
        [Test]
        public void WarnLog()
        {
            Log.Warn($"test");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Write error log
        /// </summary>
        [Test]
        public void ErrorLog()
        {
            Log.Error($"test");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Write fatal log
        /// </summary>
        [Test]
        public void FatalLog()
        {
            Log.Fatal($"test");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Check if log file even exists after writing multiple logs
        /// </summary>
        [Test]
        public void LogFileExists()
        {
            bool exists = File.Exists(Path.Combine(Data.Path, Data.LogFile));
            Assert.IsTrue(exists);
        }

        /// <summary>
        /// Write 300 different logs
        /// </summary>
        [Test]
        public void MassLog()
        {
            for (int count = 0; count < 50; count++)
            {
                Log.Debug($"debug {count}");
                Log.Trace($"trace {count}");
                Log.Info($"info {count}");
                Log.Warn($"warning {count}");
                Log.Error($"error {count}");
                Log.Fatal($"fatal {count}");
            }
        }

        /// <summary>
        /// Check if mass log created multiple log files
        /// </summary>
        [Test]
        public void LogFilesExists()
        {
            bool exists = false;
            if (Directory.GetFiles(Data.Path).Count() > 0)
                exists = true;

            Assert.IsTrue(exists);
        }
    }
}
