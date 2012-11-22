using System.Text.RegularExpressions;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;
    using Actions;
    using Messages;
    using Microsoft.Practices.ObjectBuilder2;

    public abstract class BaseRule
    {
        private readonly IActionFactory actionFactory;
        private IEnumerable<IAction> actions;
        protected Regex subjectRegex;

        protected BaseRule(IActionFactory actionFactory)
        {
            this.actionFactory = actionFactory;
        }

        public virtual void Initialize(RuleEntry ruleEntry)
        {
            this.Name = ruleEntry.Name;

            this.subjectRegex = new Regex(ruleEntry.SubjectRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public string Name { get; private set; }

        public void RetrieveActions(ILogger logger)
        {
            logger.Log(LogLevel.Information,"Creating Actions");
            this.actions = this.actionFactory.CreateActions(this.Name);
        }

        public void Process(IMessage message, ILogger logger)
        {
            logger.Log(LogLevel.Information,"Executing actions");
            this.actions.ForEach(a => a.Execute(message,logger));
        }

        public abstract bool IsMatch(IMessage message, bool previouslyMatched);
    }
}
