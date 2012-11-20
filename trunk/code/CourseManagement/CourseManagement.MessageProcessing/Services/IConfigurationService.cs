namespace CourseManagement.MessageProcessing.Services
{
    public interface IConfigurationService
    {
        string GetValue(string rulesConfigurationFilePathKey);

        string AttachmentsRootPath { get; }
        string LogsRootPath { get; }

        int MonitoredSubjectId { get; }

        string IncomingMessageProtocol { get; }
        string OutgoingMessageProtocol { get; }
    }
}
