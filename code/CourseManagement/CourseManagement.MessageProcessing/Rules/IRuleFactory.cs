using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;

    public interface IRuleFactory
    {
        IEnumerable<BaseRule> CreateRules(ILogger logger);
    }
}
