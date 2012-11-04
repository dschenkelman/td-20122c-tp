namespace CourseManagement.MessageProcessing.Services
{
    public interface IConfigurationService
    {
        string GetValue(string rulesConfigurationFilePathKey);
        string RootPath { get; set; }
        int MonitoringCourseSubjectId { get; set; }
        string MonitoringCourseIncomingMessageProtocol { get; set; }
    }
}
