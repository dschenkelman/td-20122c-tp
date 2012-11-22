namespace CourseManagement.MessageProcessing.Rules
{
    using System.Linq;
    using Actions;
    using Messages;
    using Persistence.Logging;
    using Persistence.Repositories;

    public class AddDeliverableToGroupDatabaseEntryRule : BaseRule
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public AddDeliverableToGroupDatabaseEntryRule(
            ICourseManagementRepositories courseManagementRepositories,
            IActionFactory actionFactory,
            ILogger logger) 
            : base(actionFactory, logger)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            if (!this.subjectRegex.IsMatch(message.Subject) || message.Attachments.Count() == 0)
            {
                return false;
            }

            return this.courseManagementRepositories
                       .Students
                       .Get(s => s.MessagingSystemId == message.From)
                       .FirstOrDefault() != null;
        }
    }
}
