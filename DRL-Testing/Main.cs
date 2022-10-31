using DR.Logging;
using static DR.Logging.Configuration;

using ByteSizeLib;

namespace DRL_Testing
{
    [SetUpFixture]
    public class Main
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Data.Path = Path.Combine(Path.GetTempPath(), "dr-logging-temp");
            Data.LogFile = "log.json";

            Directory.CreateDirectory(Data.Path);
            _ = new Configuration(false, Path.Combine(Data.Path, Data.LogFile), (int)ByteSize.FromKiloBytes(25).Bytes, TimeZoneInfo.Local, null);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Directory.Delete(Data.Path, true);
        }
    }
}