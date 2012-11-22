using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.Unity;

    public class UnityRuleFactory : IRuleFactory
    {
        private readonly IUnityContainer container;
        private readonly IRuleFinder ruleFinder;

        public UnityRuleFactory(IUnityContainer container, IRuleFinder ruleFinder)
        {
            this.container = container;
            this.ruleFinder = ruleFinder;
        }

        public IEnumerable<BaseRule> CreateRules(ILogger logger)
        {
            logger.Log(LogLevel.Information,"Finding Rules");

            IEnumerable<RuleEntry> rules = this.ruleFinder.FindRules();

            logger.Log(LogLevel.Information, "Initializing Rules and Retrieving Actions");
            return rules.Select(ruleEntry =>
                {
                    var ruleName = ruleEntry.Name;

                    if (ruleName.Contains("-"))
                    {
                        //Ticket rule
                        ruleName = ruleName.Split('-').First();
                    }

                    var rule = this.container.Resolve<BaseRule>(ruleName);
                    rule.Initialize(ruleEntry);
                    rule.RetrieveActions( logger );
                    return rule;
                });
        }
    }
}