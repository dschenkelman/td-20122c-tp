namespace CourseManagement.Messages
{
    using System;
    using System.Collections.Generic;

    public interface IMessage
    {
        string Subject { get; }

        string From { get; }

        string To { get; }

        DateTime Date { get; }

        IEnumerable<IMessageAttachment> Attachments { get; }
    }
}
