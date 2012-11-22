using System;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Actions
{
    using System.Text.RegularExpressions;
    using Messages;

    public abstract class BaseTicketReplyAction : IAction
    {
        private const string SubjectPattern = @"^\[CONSULTA-(?<ticketId>[0-9]+)\].*$";

        private readonly Regex subjectRegex;

        protected BaseTicketReplyAction()
        {
            this.subjectRegex = new Regex(SubjectPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public abstract void Initialize(ActionEntry actionEntry);

        public abstract void Execute(IMessage message);

        protected int ParseTicketId(IMessage message)
        {
            Match match = this.subjectRegex.Match(message.Subject);
            return Convert.ToInt32(match.Groups["ticketId"].Value);
        }
    }
}
