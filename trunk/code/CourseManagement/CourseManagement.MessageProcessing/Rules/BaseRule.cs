namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;
    using Actions;
    using Microsoft.Practices.ObjectBuilder2;

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

        public void Process(IMessage message)
        {
            this.actions.ForEach(a => a.Execute(message));
        }
    }
}
