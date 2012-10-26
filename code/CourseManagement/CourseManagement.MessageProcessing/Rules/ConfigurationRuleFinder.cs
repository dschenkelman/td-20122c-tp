namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;
    using Services;

    public class ConfigurationRuleFinder : IRuleFinder
    {
        private const string RulesConfigurationFilePathKey = "RulesConfigurationFilePath";

        private readonly IConfigurationService configurationService;
        private readonly IXmlRuleReader xmlRuleReader;

        public ConfigurationRuleFinder(IConfigurationService configurationService, IXmlRuleReader xmlRuleReader)
        {
            this.configurationService = configurationService;
            this.xmlRuleReader = xmlRuleReader;
        }

        public IEnumerable<string> FindNames()
        {
            string rulesConfigurationFilePath = 
                this.configurationService.GetValue(RulesConfigurationFilePathKey);

            return this.xmlRuleReader.GetRuleNames(rulesConfigurationFilePath);
        }
    }
}
