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
            IEnumerable<string> ruleNames = this.ruleFinder.FindNames();

            return ruleNames.Select(rn =>
                {
                    var rule = this.container.Resolve<BaseRule>(rn);
                    rule.Name = rn;
                    rule.RetrieveActions();
                    return rule;
                });
        }
    }
}