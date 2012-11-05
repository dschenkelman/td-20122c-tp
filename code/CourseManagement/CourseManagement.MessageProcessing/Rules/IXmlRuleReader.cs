namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;

    public interface IXmlRuleReader
    {
        IEnumerable<RuleEntry> GetRuleNames(string rulesConfigurationFilePath);
    }
}
