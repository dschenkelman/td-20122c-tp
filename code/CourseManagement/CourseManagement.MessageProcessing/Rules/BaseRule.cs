namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;
    using Microsoft.Practices.ObjectBuilder2;
    using Persistence.Logging;

    public abstract class BaseRule
    {
        private readonly IActionFactory actionFactory;
        private readonly ILogger logger;
        private IEnumerable<IAction> actions;
        protected Regex subjectRegex;

        protected BaseRule(IActionFactory actionFactory, ILogger logger)
        {
            this.actionFactory = actionFactory;
            this.logger = logger;
        }

        public virtual void Initialize(RuleEntry ruleEntry)
        {
            this.Name = ruleEntry.Name;

            this.subjectRegex = new Regex(ruleEntry.SubjectRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public string Name { get; private set; }

        public void RetrieveActions()
        {
            this.logger.Log(LogLevel.Information, "Retrieving Actions for Rule: "+this.Name);
            this.actions = this.actionFactory.CreateActions(this.Name);
        }

        public void Process(IMessage message)
        {
            this.logger.Log(LogLevel.Information, "Executing actions");
            this.actions.ForEach(a => a.Execute(message));
        }

        public abstract bool IsMatch(IMessage message, bool previouslyMatched);
    }
}
