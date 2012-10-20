namespace CourseManagement.EmailProcessing.Services
{
    public interface IConfigurationService
    {
        string GetValue(string rulesConfigurationFilePathKey);
    }
}
