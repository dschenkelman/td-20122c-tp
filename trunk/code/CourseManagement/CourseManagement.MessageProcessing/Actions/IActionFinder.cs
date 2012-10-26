namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;

    public interface IActionFinder
    {
        IEnumerable<string> FindNames(string ruleName);
    }
}
