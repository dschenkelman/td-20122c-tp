using System;
using CourseManagement.Utilities.Extensions;

namespace CourseManagement.Persistence.Logging
{
    using System.IO;
    using Configuration;

    public class FileLogger : ILogger
    {
        private const string LogFilePathKey = "LogFile";

        private string logFilePath;
        private object lockObject = new object();

        public FileLogger(IConfigurationService configurationService)
        {
            this.logFilePath = configurationService.GetValue(LogFilePathKey);
        }

        public void Log(LogLevel information, string message)
        {
            lock (this.lockObject)
            {
                using (var w = File.AppendText(this.logFilePath))
                {
                    w.WriteLine(string.Format("{0};{1};{2}", DateTime.Now.ToIsoFormat(), information, message));
                }
            }
        }
    }
}
