namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;

    public interface IRuleFinder
    {
        IEnumerable<RuleEntry> FindRules();
    }
}
