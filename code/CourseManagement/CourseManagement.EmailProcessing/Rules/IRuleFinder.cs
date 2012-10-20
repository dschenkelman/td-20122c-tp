namespace CourseManagement.EmailProcessing.Rules
{
    using System.Collections.Generic;

    public interface IRuleFinder
    {
        IEnumerable<string> FindNames();
    }
}
