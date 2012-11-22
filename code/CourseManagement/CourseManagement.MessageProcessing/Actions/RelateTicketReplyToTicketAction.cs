using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Actions
{
    using Messages;
    using Model;
    using Persistence.Repositories;

    public class RelateTicketReplyToTicketAction : BaseTicketReplyAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public RelateTicketReplyToTicketAction(ICourseManagementRepositories courseManagementRepositories)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override void Initialize(ActionEntry actionEntry)
        {
        }

        public override void Execute(IMessage message, ILogger logger)
        {
            int ticketId = this.ParseTicketId(message);

            logger.Log(LogLevel.Information,"Obtaining Ticket ID");

            Ticket ticket = this.courseManagementRepositories.Tickets.GetById(ticketId);

            logger.Log(LogLevel.Information, "Generating a new Ticket Reply");

            Reply reply = new Reply
                              {
                                  DateCreated = message.Date, 
                                  MessageBody = message.Body,
                                  MessageSubject = message.Subject
                              };

            ticket.Replies.Add(reply);
            this.courseManagementRepositories.Tickets.Save();
        }
    }
}
