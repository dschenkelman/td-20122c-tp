using System.Collections.Generic;
using System.Linq;

namespace CourseManagement.Messages.Tests
{
    using System.IO.Moles;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EmailAttachmentFixture
    {
        [TestMethod]
        [HostType("Moles")]
        public void ShouldWriteStreamContentToDiskWhenDownloadingToPath()
        {
            // arrange
            const string AttachmentName = "Attachment";
            const long StreamLength = 20;
            const string DownloadPath = "FakePath";

            byte[] bytes = new byte[StreamLength];
            for (int i = 0; i < StreamLength; i++)
            {
                bytes[i] = (byte)i;
            }

            SStream streamStub = new SStream();
            streamStub.LengthGet = () => StreamLength;
            streamStub.ReadByteArrayInt32Int32 = (content, offset, length) =>
                                                     {
                                                         Assert.AreEqual(0, offset);
                                                         Assert.AreEqual(StreamLength, 20);

                                                         return (int)StreamLength;
                                                     };

            bool writeAllBytesInvoked = false;
            MFile.WriteAllBytesStringByteArray = (s, b) =>
                                                     {
                                                         Assert.AreEqual(DownloadPath, s);
                                                         writeAllBytesInvoked = true;
                                                     };

            EmailAttachment attachment = new EmailAttachment(AttachmentName, () => streamStub);
            
            // act
            attachment.Download(DownloadPath);

            // assert
            Assert.IsTrue(writeAllBytesInvoked);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldReturnLinesFromStreamReaderWhenReadingStream()
        {
            // arrange
            SStream streamStub = new SStream();
            
            Queue<string> lines = new Queue<string>();
            lines.Enqueue("Line 1");
            lines.Enqueue("Line 2");
            lines.Enqueue("Line 3");

            MStreamReader.ConstructorStream = (reader, stream) => { Assert.AreSame(streamStub, stream); };
            MStreamReader.AllInstances.ReadLine = reader => { return lines.Dequeue(); };
            MStreamReader.AllInstances.EndOfStreamGet = reader => { return lines.Count == 0; };

            const string AttachmentName = "Attachment";
            EmailAttachment attachment = new EmailAttachment(AttachmentName, () => streamStub);

            // act
            var retrievedLines = attachment.RetrieveLines().ToList();

            // assert
            Assert.AreEqual(retrievedLines[0], "Line 1");
            Assert.AreEqual(retrievedLines[1], "Line 2");
            Assert.AreEqual(retrievedLines[2], "Line 3");
        }
    }
}
