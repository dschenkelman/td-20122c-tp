namespace CourseManagement.MessageProcessing
{
    using System;

    public interface IMessage
    {
        string Subject { get; set; }
        string Address { get; set; }
        string DestinationAddress { get; set; }
        DateTime Date { get; set; }
    }
}
