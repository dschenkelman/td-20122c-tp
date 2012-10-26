namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;

    public interface IActionFactory
    {
        IEnumerable<IAction> CreateActions(string ruleName);
    }
}
