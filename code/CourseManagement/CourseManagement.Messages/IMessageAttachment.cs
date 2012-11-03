namespace CourseManagement.Messages
{
    using System.Collections.Generic;

    public interface IMessageAttachment
    {
        string Name { get; set; }

        void Download(string path);

        IEnumerable<string> RetrieveLines();
    }
}
