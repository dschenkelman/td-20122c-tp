﻿using System.Text.RegularExpressions;

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

        public void RetrieveActions()
        {
            this.actions = this.actionFactory.CreateActions(this.Name);
        }

        public void Process(IMessage message)
        {
            this.actions.ForEach(a => a.Execute(message));
        }

        public abstract bool IsMatch(IMessage message, bool previouslyMatched);
    }
}
