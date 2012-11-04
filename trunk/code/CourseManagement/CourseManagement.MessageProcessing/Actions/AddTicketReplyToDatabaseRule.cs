using System;

namespace CourseManagement.MessageProcessing.Actions
{
    using System.Text.RegularExpressions;
    using Messages;
    using Persistence.Repositories;
    using Rules;

    public class AddTicketReplyToDatabaseRule : BaseRule
    {
        private const string SubjectPattern = @"^\[CONSULTA-(?<ticketId>[0-9]+)\].*$";
        
        private readonly ICourseManagementRepositories courseManagementRepositories;

        private readonly Regex subjectRegex;

        public AddTicketReplyToDatabaseRule(
            IActionFactory actionFactory, 
            ICourseManagementRepositories courseManagementRepositories) : base(actionFactory)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.subjectRegex = new Regex(SubjectPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public override bool IsMatch(IMessage message)
        {
            Match match = this.subjectRegex.Match(message.Subject);
            if (!match.Success)
            {
                return false;
            }

            int ticketId;
            if (!int.TryParse(match.Groups["ticketId"].Value, out ticketId))
            {
                return false;
            }

            return this.courseManagementRepositories.Tickets.GetById(ticketId) != null;
        }
    }
}
