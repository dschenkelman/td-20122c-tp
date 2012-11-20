using CourseManagement.Persistence.Configuration;

namespace CourseManagement.MessageProcessing.Actions
{
    using System.Linq;
    using Messages;
    using Persistence.Repositories;

    public class NewTicketEmailReplyAction : CreateEmailReplyAction
    {
        public NewTicketEmailReplyAction(
            IMessageSender messageSender, 
            ICourseManagementRepositories courseManagementRepositories,
            IConfigurationService configurationService) 
            : base(messageSender, courseManagementRepositories, configurationService)
        {
        }

        protected override string GenerateSubject(IMessage receivedMessage)
        {
            string baseSubject = base.GenerateSubject(receivedMessage);

            var ticket = this.CourseManagementRepositories.Tickets.Get(
                t => t.DateCreated == receivedMessage.Date && t.MessageSubject == receivedMessage.Subject).FirstOrDefault();

            return string.Format(baseSubject, ticket.Id);
        }
    }
}
