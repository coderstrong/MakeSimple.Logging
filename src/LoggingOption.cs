using System.ComponentModel;
using System.IO;

namespace MakeSimple.Logging
{
    public enum LoggerLevel
    {
        [Description("Information")]
        Information,

        [Description("Verbose")]
        Verbose,

        [Description("Debug")]
        Debug,

        [Description("Error")]
        Error,

        [Description("Fatal")]
        Fatal,

        [Description("Warning")]
        Warning
    }

    public class LoggingOption
    {
        public bool IsEnableTracing { get; set; }
        public bool IsLogConsole { get; set; }
        public bool IsOffLogSystem { get; set; }
        public long FileSizeLimit { get; set; }
        public string Path { get; set; }
        public LoggerLevel MinimumLevel { get; set; }

        /// <summary>
        /// Default
        /// LogConsole = True
        /// FileSizeLimit = Default
        /// MinimumLevel = Information
        /// OffLogSystem = True
        /// </summary>
        public LoggingOption()
        {
            IsEnableTracing = true;
            IsOffLogSystem = true;
            IsLogConsole = true;
            FileSizeLimit = 52428800; // 50MB
            Path = Directory.GetCurrentDirectory();
            MinimumLevel = LoggerLevel.Information;
        }
    }
}