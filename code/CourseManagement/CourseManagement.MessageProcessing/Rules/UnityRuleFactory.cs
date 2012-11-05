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

        public IEnumerable<BaseRule> CreateRules()
        {
            IEnumerable<RuleEntry> rules = this.ruleFinder.FindRules();

            return rules.Select(ruleEntry =>
                {
                    var rule = this.container.Resolve<BaseRule>(ruleEntry.Name);
                    rule.Initialize(ruleEntry);
                    rule.RetrieveActions();
                    return rule;
                });
        }
    }
}