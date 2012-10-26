namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;
    using Services;

    public class ConfigurationActionFinder : IActionFinder
    {
       private const string RulesConfigurationFilePathKey = "RulesConfigurationFilePath";

        private readonly IConfigurationService configurationService;
        private readonly IXmlActionReader xmlActionReader;

        public ConfigurationActionFinder(IConfigurationService configurationService, IXmlActionReader xmlRuleReader)
        {
            this.configurationService = configurationService;
            this.xmlActionReader = xmlRuleReader;
        }

        public IEnumerable<string> FindNames(string ruleName)
        {
            string rulesConfigurationFilePath = 
                this.configurationService.GetValue(RulesConfigurationFilePathKey);

            return this.xmlActionReader.GetActionNames(rulesConfigurationFilePath, ruleName);
        }
    }
}
