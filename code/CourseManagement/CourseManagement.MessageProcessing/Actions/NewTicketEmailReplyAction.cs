using CourseManagement.Persistence.Configuration;
using CourseManagement.Persistence.Logging;

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
            IConfigurationService configurationService,
            ILogger logger) 
            : base(messageSender, courseManagementRepositories, configurationService, logger)
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
