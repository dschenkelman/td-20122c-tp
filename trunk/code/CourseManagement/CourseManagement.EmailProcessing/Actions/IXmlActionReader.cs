using System.Collections.Generic;

namespace CourseManagement.EmailProcessing.Actions
{
    public interface IXmlActionReader
    {
        IEnumerable<string> GetActionNames(string rulesConfigurationFilePath, string ruleName);
    }
}
