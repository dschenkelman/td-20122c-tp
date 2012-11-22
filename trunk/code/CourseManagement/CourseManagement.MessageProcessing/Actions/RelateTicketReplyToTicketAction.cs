namespace CourseManagement.MessageProcessing.Actions
{
    using Messages;
    using Model;
    using Persistence.Logging;
    using Persistence.Repositories;

    public class RelateTicketReplyToTicketAction : BaseTicketReplyAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;
        private readonly ILogger logger;

        public RelateTicketReplyToTicketAction(
            ICourseManagementRepositories courseManagementRepositories,
            ILogger logger)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.logger = logger;
        }

        public override void Initialize(ActionEntry actionEntry)
        {
        }

        public override void Execute(IMessage message)
        {
            int ticketId = this.ParseTicketId(message);

            this.logger.Log(LogLevel.Information, "Retrieving Ticket ID");

            Ticket ticket = this.courseManagementRepositories.Tickets.GetById(ticketId);

            this.logger.Log(LogLevel.Information, "Generating a new Ticket Reply");

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
