﻿using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;
    using Persistence.Repositories;

    internal class AddNewTicketToDatabaseRule : BaseRule
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public AddNewTicketToDatabaseRule(
            IActionFactory actionFactory, 
            ICourseManagementRepositories courseManagementRepositories,
            ILogger logger)
            : base(actionFactory, logger)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            return this.subjectRegex.IsMatch(message.Subject);
        }
    }
}
