using System;
using System.Linq;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Utilities.Extensions;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;
    using Persistence.Repositories;

    public class AddTicketReplyToDatabaseRule : BaseRule
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;
        private readonly IConfigurationService configurationService;

        public AddTicketReplyToDatabaseRule(
            IActionFactory actionFactory, 
            ICourseManagementRepositories courseManagementRepositories, 
            IConfigurationService configurationService) : base(actionFactory)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.configurationService = configurationService;
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
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

            int semester = message.Date.Semester();
            int year = message.Date.Year;
            int subjectId = this.configurationService.MonitoredSubjectId;
            var course = this.courseManagementRepositories.Courses
                .Get(c => c.Semester == semester && c.Year == year && subjectId == c.SubjectId)
                .FirstOrDefault();

            if (!(course.Students.Any(s => s.MessagingSystemId == message.From) || course.Teachers.Any(t => t.MessagingSystemId == message.From)))
            {
                throw new InvalidOperationException("Cannot reply to ticket when not registered in course");
            }

            return this.courseManagementRepositories.Tickets.GetById(ticketId) != null;
        }
    }
}
