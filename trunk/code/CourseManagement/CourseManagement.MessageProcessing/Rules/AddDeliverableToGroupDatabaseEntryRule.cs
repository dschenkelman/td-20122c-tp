using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;

    class AddDeliverableToGroupDatabaseEntryRule : BaseRule
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public AddDeliverableToGroupDatabaseEntryRule(ICourseManagementRepositories courseManagementRepositories, IActionFactory actionFactory) : base(actionFactory)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            if( ! Regex.IsMatch(message.Subject, @"^\[ENTREGA-TP-[0-9]+\]$") || message.Attachments.Count() == 0 )
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
