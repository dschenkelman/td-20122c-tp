namespace CourseManagement.EmailProcessing.Services
{
    using System.Configuration;

    public class ConfigurationService : IConfigurationService
    {
        public string GetValue(string rulesConfigurationFilePathKey)
        {
            return ConfigurationManager.AppSettings[rulesConfigurationFilePathKey];
        }
    }
}
