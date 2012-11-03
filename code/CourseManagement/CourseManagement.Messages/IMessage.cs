namespace CourseManagement.Messages
{
    using System;
    using System.Collections.Generic;

    public interface IMessage
    {
        string Subject { get; }

        string Body { get; }

        string From { get; }

        ICollection<string> To { get; }

        DateTime Date { get; }

        IEnumerable<IMessageAttachment> Attachments { get; }
    }
}
