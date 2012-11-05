using System;
using System.Text.RegularExpressions;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing.Actions
{
    using Messages;

    public class RelateTicketReplyToTicketAction : IAction
    {
        private const string SubjectPattern = @"^\[CONSULTA-(?<ticketId>[0-9]+)\].*$";
        
        private readonly ICourseManagementRepositories courseManagementRepositories;
        private readonly Regex subjectRegex;

        public RelateTicketReplyToTicketAction(ICourseManagementRepositories courseManagementRepositories)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.subjectRegex = new Regex(SubjectPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public void Initialize(ActionEntry actionEntry)
        {
        }

        public void Execute(IMessage message)
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

        private int ParseTicketId(IMessage message)
        {
            Match match = this.subjectRegex.Match(message.Subject);
            return Convert.ToInt32(match.Groups["ticketId"].Value);
        }
    }
}
