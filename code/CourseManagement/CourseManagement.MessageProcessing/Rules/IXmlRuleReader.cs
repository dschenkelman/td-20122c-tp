namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;

    public interface IXmlRuleReader
    {
        IEnumerable<string> GetRuleNames(string rulesConfigurationFilePath);
    }
}
