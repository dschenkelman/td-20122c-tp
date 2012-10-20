namespace CourseManagement.EmailProcessing.Actions
{
    using System.Collections.Generic;

    public interface IXmlActionReader
    {
        IEnumerable<string> GetActionNames(string rulesConfigurationFilePath, string ruleName);
    }
}
