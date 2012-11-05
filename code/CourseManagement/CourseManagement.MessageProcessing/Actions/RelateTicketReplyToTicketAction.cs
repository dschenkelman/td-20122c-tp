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

        public override void Execute(IMessage message)
        {
            int ticketId = this.ParseTicketId(message);

            Ticket ticket = this.courseManagementRepositories.Tickets.GetById(ticketId);

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
