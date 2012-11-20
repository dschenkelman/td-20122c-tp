namespace CourseManagement.Persistence.Logging
{
    public interface ILogger
    {
        void Log(LogLevel information, string message);
    }
}
