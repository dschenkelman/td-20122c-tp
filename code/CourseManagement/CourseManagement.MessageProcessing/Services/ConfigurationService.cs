using System.Configuration;

namespace CourseManagement.MessageProcessing.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetValue(string rulesConfigurationFilePathKey)
        {
            return ConfigurationManager.AppSettings[rulesConfigurationFilePathKey];
        }
    }
}
