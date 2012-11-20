namespace CourseManagement.Persistence.Logging
{
    using Configuration;

    public class FileLogger : ILogger
    {
        private const string LogFilePathKey = "LogFile";

        private string logFilePath;

        public FileLogger(IConfigurationService configurationService)
        {
            this.logFilePath = configurationService.GetValue(LogFilePathKey);
        }

        public void Log(LogLevel information, string message)
        {
        }
    }
}
