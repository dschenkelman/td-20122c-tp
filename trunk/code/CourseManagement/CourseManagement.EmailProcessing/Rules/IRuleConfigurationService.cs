namespace CourseManagement.EmailProcessing.Rules
{
    using System.Collections.Generic;

    public interface IRuleConfigurationService
    {
        IEnumerable<string> GetRuleNames(string rulesConfigurationFilePath);
    }
}
