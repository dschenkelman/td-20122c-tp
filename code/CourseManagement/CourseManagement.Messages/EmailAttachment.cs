namespace CourseManagement.Messages
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class EmailAttachment : IMessageAttachment
    {
        private readonly Func<Stream> getAttachmentStream;

        public EmailAttachment(string name, Func<Stream> getAttachmentStreamAction)
        {
            this.getAttachmentStream = getAttachmentStreamAction;
            this.Name = name;
        }

        public string Name { get; set; }

        public void Download(string path)
        {
            Stream fileStream = this.getAttachmentStream();

            byte[] content = new byte[fileStream.Length];

            fileStream.Read(content, 0, (int)fileStream.Length);

            File.WriteAllBytes(path, content);
        }

        public IEnumerable<string> RetrieveLines()
        {
            using (StreamReader streamReader = new StreamReader(this.getAttachmentStream()))
            {
                yield return streamReader.ReadLine();
            }
        }
    }
}