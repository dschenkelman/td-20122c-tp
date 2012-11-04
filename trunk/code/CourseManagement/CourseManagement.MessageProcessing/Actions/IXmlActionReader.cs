namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;

    public interface IXmlActionReader
    {
        IEnumerable<ActionEntry> GetActionEntries(string rulesConfigurationFilePath, string ruleName);
    }
}
