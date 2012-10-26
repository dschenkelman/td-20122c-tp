namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;

    public interface IRuleFactory
    {
        IEnumerable<BaseRule> CreateRules();
    }
}
