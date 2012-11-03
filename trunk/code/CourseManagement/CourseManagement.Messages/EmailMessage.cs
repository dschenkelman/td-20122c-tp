namespace CourseManagement.Messages
{
    using System;
    using System.Collections.Generic;
    
    public class EmailMessage : IMessage
    {
        public EmailMessage(string subject, string from, string to, DateTime date, IEnumerable<IMessageAttachment> attachments)
        {
            this.Subject = subject;
            this.From = from;
            this.To = to;
            this.Date = date;
            this.Attachments = attachments;
        }

        public string Subject { get; private set; }

        public string From { get; private set; }

        public string To { get; private set; }

        public DateTime Date { get; private set; }

        public IEnumerable<IMessageAttachment> Attachments { get; private set; }
    }
}
