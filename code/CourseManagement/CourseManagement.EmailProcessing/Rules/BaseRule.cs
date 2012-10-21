using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

namespace CourseManagement.EmailProcessing.Rules
{
    using Actions;

    public abstract class BaseRule
    {
        private readonly IActionFactory actionFactory;
        private IEnumerable<IAction> actions;

        protected BaseRule(IActionFactory actionFactory)
        {
            this.actionFactory = actionFactory;
        }

        public string Name { get; set; }

        public void RetrieveActions()
        {
            this.actions = this.actionFactory.CreateActions(this.Name);
        }

        public void Process(IEmail email)
        {
            this.actions.ForEach(a => a.Execute(email));
        }
    }
}
