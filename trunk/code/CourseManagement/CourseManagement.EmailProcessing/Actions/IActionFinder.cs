using System.Collections.Generic;

namespace CourseManagement.EmailProcessing.Actions
{
    public interface IActionFinder
    {
        IEnumerable<string> FindNames(string ruleName);
    }
}
