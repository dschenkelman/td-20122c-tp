namespace CourseManagement.EmailProcessing.Rules
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

        public IEnumerable<IRule> CreateRules()
        {
            IEnumerable<string> ruleNames = this.ruleFinder.FindNames();

            return ruleNames.Select(r => this.container.Resolve<IRule>(r));
        }
    }
}