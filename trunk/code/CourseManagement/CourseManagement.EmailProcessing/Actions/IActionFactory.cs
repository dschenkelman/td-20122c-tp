namespace CourseManagement.EmailProcessing.Actions
{
    using System.Collections.Generic;

    public interface IActionFactory
    {
        IEnumerable<IAction> CreateActions(string ruleName);
    }
}
