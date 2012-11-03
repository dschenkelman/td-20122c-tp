namespace CourseManagement.Messages
{
    using System;
    using System.Collections.Generic;
    
    public class EmailMessage : IMessage
    {
        public EmailMessage(string subject, string from, DateTime date, IEnumerable<IMessageAttachment> attachments, params string[] to)
        {
            this.Subject = subject;
            this.From = from;
            this.To = new List<string>();
            Array.ForEach(to, this.To.Add);
            this.Date = date;
            this.Attachments = attachments;
        }

        public string Subject { get; private set; }

        public string Body { get; private set; }

        public string From { get; private set; }

        public ICollection<string> To { get; private set; }

        public DateTime Date { get; private set; }

        public IEnumerable<IMessageAttachment> Attachments { get; private set; }
    }
}
