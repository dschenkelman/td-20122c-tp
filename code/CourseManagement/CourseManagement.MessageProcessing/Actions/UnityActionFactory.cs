namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.Unity;

    public class UnityActionFactory : IActionFactory
    {
        private readonly IActionFinder actionFinder;
        private readonly IUnityContainer container;

        public UnityActionFactory(IUnityContainer container, IActionFinder actionFinder)
        {
            this.actionFinder = actionFinder;
            this.container = container;
        }

        public IEnumerable<IAction> CreateActions(string ruleName)
        {
            IEnumerable<ActionEntry> actions = this.actionFinder.FindActions(ruleName);

            return actions.Select(actionEntry =>
                {
                    var action = this.container.Resolve<IAction>(actionEntry.Name);
                    action.Initialize(actionEntry);
                    return action;
                });
        }
    }
}
