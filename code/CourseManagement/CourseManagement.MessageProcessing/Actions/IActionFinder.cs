namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;

    public interface IActionFinder
    {
        IEnumerable<ActionEntry> FindActions(string ruleName);
    }
}
