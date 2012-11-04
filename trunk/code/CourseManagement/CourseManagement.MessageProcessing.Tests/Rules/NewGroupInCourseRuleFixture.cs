using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.MessageProcessing.Rules;
using CourseManagement.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    [TestClass]
    public class NewGroupInCourseRuleFixture
    {
        private MockRepository mockRepository;
        private Mock<IActionFactory> actionFactory;
        private Mock<IMessage> message;
        private Mock<IMessageAttachment> MessageAttachment;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.message = this.mockRepository.Create<IMessage>();
            this.MessageAttachment = this.mockRepository.Create<IMessageAttachment>();
        }

        [TestMethod]
        public void ShouldMatchMessage()
        {
            // arrange

            const string messageSubject = "[ALTA-GRUPO]";

            MessageAttachment.Setup(ma => ma.Name).Returns("attachment.txt").Verifiable();

            var attachmentList = new List<IMessageAttachment> { MessageAttachment.Object };

            message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsTrue(resultado);
            MessageAttachment.Verify(ma => ma.Name , Times.Once());
            message.Verify(m => m.Attachments, Times.Exactly(2));
            message.Verify(m => m.Subject, Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithWrongMessageSubject()
        {
            // arrange

            const string messageSubject = "WRONG-SUBJECT";

            MessageAttachment.Setup(ma => ma.Name).Returns("attachment.txt").Verifiable();

            var attachmentList = new List<IMessageAttachment> { MessageAttachment.Object };

            message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            MessageAttachment.Verify(ma => ma.Name, Times.Once());
            message.Verify(m => m.Attachments, Times.Exactly(2));
            message.Verify(m => m.Subject, Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithToManyAttachments()
        {
            // arrange

            const string messageSubject = "[ALTA-GRUPO]";

            MessageAttachment.Setup(ma => ma.Name).Returns("attachment.txt").Verifiable();

            var messageAttachment2 = this.mockRepository.Create<IMessageAttachment>();
            messageAttachment2.Setup(ma => ma.Name).Returns("attachment2.txt").Verifiable();

            var attachmentList = new List<IMessageAttachment> { MessageAttachment.Object, messageAttachment2.Object };

            message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            MessageAttachment.Verify(ma => ma.Name, Times.Never());
            message.Verify(m => m.Attachments, Times.Once());
            message.Verify(m => m.Subject, Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithWrongExtension()
        {
            // arrange

            const string messageSubject = "[ALTA-GRUPO]";

            this.MessageAttachment.Setup(ma => ma.Name).Returns("attachment.ext1").Verifiable();

            var attachmentList = new List<IMessageAttachment> { MessageAttachment.Object };

            this.message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            this.message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            this.MessageAttachment.Verify(ma => ma.Name, Times.Once());
            this.message.Verify(m => m.Attachments, Times.Exactly(2));
            this.message.Verify(m => m.Subject, Times.Once());
        }

        private NewGroupInCourseRule CreateRule()
        {
            return new NewGroupInCourseRule(this.actionFactory.Object);
        }
    }
}
