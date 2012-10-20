namespace CourseManagement.EmailProcessing.Rules
{
    using System.Collections.Generic;

    public interface IRuleFactory
    {
        IEnumerable<BaseRule> CreateRules();
    }
}
