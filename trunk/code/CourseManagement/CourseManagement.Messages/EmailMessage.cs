namespace CourseManagement.Messages
{
    using System;
    using System.Collections.Generic;
    
    public class EmailMessage : IMessage
    {
        public string Subject { get; private set; }

        public string From { get; private set; }

        public string To { get; private set; }

        public DateTime Date { get; set; }

        public IEnumerable<IMessageAttachment> Attachments { get; set; }
    }
}
