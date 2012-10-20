namespace CourseManagement.EmailProcessing.Rules
{
    using System.Collections.Generic;

    public interface IXmlRuleReader
    {
        IEnumerable<string> GetRuleNames(string rulesConfigurationFilePath);
    }
}
