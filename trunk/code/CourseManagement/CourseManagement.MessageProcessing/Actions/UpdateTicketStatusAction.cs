namespace CourseManagement.MessageProcessing.Actions
{
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Services;
    using Utilities.Extensions;

    public class UpdateTicketStatusAction : BaseTicketReplyAction
    {
        private readonly IConfigurationService configurationService;
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public UpdateTicketStatusAction(IConfigurationService configurationService, ICourseManagementRepositories courseManagementRepositories) 
            : base()
        {
            this.configurationService = configurationService;
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override void Initialize(ActionEntry actionEntry)
        {
        }

        public override void Execute(IMessage message)
        {
            int semester = message.Date.Semester();
            int year = message.Date.Year;
            int subjectId = this.configurationService.MonitoredSubjectId;
            var course = this.courseManagementRepositories.Courses
                .Get(c => c.Semester == semester && c.Year == year && subjectId == c.SubjectId)
                .FirstOrDefault();

            var teacher = course.Teachers.FirstOrDefault(t => t.MessagingSystemId == message.From);

            int ticketId = this.ParseTicketId(message);
            var ticket = this.courseManagementRepositories.Tickets.GetById(ticketId);

            if (teacher != null)
            {
                ticket.State = TicketState.Pending;
                ticket.TeacherId = teacher.Id;
            }
            else if (ticket.State == TicketState.Pending)
            {
                ticket.State = TicketState.Assigned;
            }

            this.courseManagementRepositories.Tickets.Save();
        }
    }
}
