using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace CourseManagement.Messages.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EmailMessageFixture
    {
        [TestMethod]
        public void ShouldParseToArgumentsFromConstructor()
        {
            // arrange
            const string Subject = "Subject";
            const string From = "From";
            DateTime date = DateTime.Today;
            Mock<IMessageAttachment> attachment1 = new Mock<IMessageAttachment>();
            Mock<IMessageAttachment> attachment2 = new Mock<IMessageAttachment>();
            List<IMessageAttachment> attachments = 
                new List<IMessageAttachment> { attachment1.Object, attachment2.Object };

            const string To1 = "To1";
            const string To2 = "To2";

            // act
            EmailMessage emailMessage = new EmailMessage(Subject, From, date, attachments, "body", To1, To2);

            // assert
            Assert.AreEqual(Subject, emailMessage.Subject);
            Assert.AreEqual(From, emailMessage.From);
            Assert.AreEqual(date, emailMessage.Date);
            Assert.AreEqual(To1, emailMessage.To.ElementAt(0));
            Assert.AreEqual(To2, emailMessage.To.ElementAt(1));
            Assert.AreSame(attachments, emailMessage.Attachments);
        }
    }
}
