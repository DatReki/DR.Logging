using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Microsoft.CSharp;

namespace DR.Logging.Models
{
    internal class LogFileModel
    {
        [JsonProperty("debug")]
        public List<LogEntryModel> Debug { get; set; } = new List<LogEntryModel>();

        [JsonProperty("trace")]
        public List<LogEntryModel> Trace { get; set; } = new List<LogEntryModel>();

        [JsonProperty("info")]
        public List<LogEntryModel> Info { get; set; } = new List<LogEntryModel>();

        [JsonProperty("warn")]
        public List<LogEntryModel> Warn { get; set; } = new List<LogEntryModel>();

        [JsonProperty("error")]
        public List<LogEntryModel> Error { get; set; } = new List<LogEntryModel>();

        [JsonProperty("fatal")]
        public List<LogEntryModel> Fatal { get; set; } = new List<LogEntryModel>();
    }

    internal class LogEntryModel
    {
        [JsonProperty("date")]
        public string Date { get; set; } = string.Empty;

        [JsonProperty("file")]
        public string File { get; set; } = string.Empty;

        [JsonProperty("method")]
        public string Method { get; set; } = string.Empty;

        [JsonProperty("line_number")]
        public int LineNumber { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }

    public partial class Deserialize<T>
    {
        public static T FromJson(string json)
        {
            dynamic? value = JsonConvert.DeserializeObject<T>(json, Converter.Settings);
            if (value == null)
                return Activator.CreateInstance<T>();
            else
                return value;
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }

    public static class Serialize
    {
        public static string ToJson<T>(this T self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
