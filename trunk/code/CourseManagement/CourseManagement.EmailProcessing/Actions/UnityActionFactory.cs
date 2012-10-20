using System;
using System.Linq;
using Microsoft.Practices.Unity;

namespace CourseManagement.EmailProcessing.Actions
{
    using System.Collections.Generic;

    public class UnityActionFactory : IActionFactory
    {
        private IActionFinder actionFinder;
        private IUnityContainer container;

        public UnityActionFactory(IUnityContainer container , IActionFinder actionFinder)
        {
            this.actionFinder = actionFinder;
            this.container = container;
        }

        public IEnumerable<IAction> CreateActions(string ruleName)
        {
            IEnumerable<string> actionsStrings = this.actionFinder.FindNames(ruleName);

            return actionsStrings.Select(actS =>
                {
                    var action = this.container.Resolve<IAction>(actS);
                    return action;
                });
        }
    }
}
