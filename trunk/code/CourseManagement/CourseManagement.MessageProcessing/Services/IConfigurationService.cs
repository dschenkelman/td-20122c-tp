namespace CourseManagement.MessageProcessing.Services
{
    public interface IConfigurationService
    {
        string GetValue(string rulesConfigurationFilePathKey);
        string RootPath { get; set; }
    }
}
