namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Persistence.Logging;

    [TestClass]
    public class NewGroupInCourseRuleFixture
    {
        private MockRepository mockRepository;
        private Mock<IActionFactory> actionFactory;
        private Mock<IMessage> message;
        private Mock<IMessageAttachment> messageAttachment;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.message = this.mockRepository.Create<IMessage>();
            this.messageAttachment = this.mockRepository.Create<IMessageAttachment>();
            this.logger = this.mockRepository.Create<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void ShouldMatchMessage()
        {
            // arrange

            const string messageSubject = "[ALTA-GRUPO]";

            messageAttachment.Setup(ma => ma.Name).Returns("attachment.txt").Verifiable();

            var attachmentList = new List<IMessageAttachment> { messageAttachment.Object };

            message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();
            rule.Initialize(new RuleEntry("NewGroup", "\\[ALTA-GRUPO\\]"));

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsTrue(resultado);
            messageAttachment.Verify(ma => ma.Name , Times.Once());
            message.Verify(m => m.Attachments, Times.Exactly(2));
            message.Verify(m => m.Subject, Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithWrongMessageSubject()
        {
            // arrange

            const string messageSubject = "WRONG-SUBJECT";

            messageAttachment.Setup(ma => ma.Name).Returns("attachment.txt").Verifiable();

            var attachmentList = new List<IMessageAttachment> { messageAttachment.Object };

            message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();
            rule.Initialize(new RuleEntry("NewGroup", "\\[ALTA-GRUPO\\]"));

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            messageAttachment.Verify(ma => ma.Name, Times.Never());
            message.Verify(m => m.Attachments, Times.Never());
            message.Verify(m => m.Subject, Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithToManyAttachments()
        {
            // arrange

            const string messageSubject = "[ALTA-GRUPO]";

            messageAttachment.Setup(ma => ma.Name).Returns("attachment.txt").Verifiable();

            var messageAttachment2 = this.mockRepository.Create<IMessageAttachment>();
            messageAttachment2.Setup(ma => ma.Name).Returns("attachment2.txt").Verifiable();

            var attachmentList = new List<IMessageAttachment> { messageAttachment.Object, messageAttachment2.Object };

            message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();
            rule.Initialize(new RuleEntry("NewGroup", "\\[ALTA-GRUPO\\]"));

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            messageAttachment.Verify(ma => ma.Name, Times.Never());
            message.Verify(m => m.Attachments, Times.Once());
            message.Verify(m => m.Subject, Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithWrongExtension()
        {
            // arrange

            const string messageSubject = "[ALTA-GRUPO]";

            this.messageAttachment.Setup(ma => ma.Name).Returns("attachment.ext1").Verifiable();

            var attachmentList = new List<IMessageAttachment> { messageAttachment.Object };

            this.message.Setup(m => m.Subject).Returns(messageSubject).Verifiable();
            this.message.Setup(m => m.Attachments).Returns(attachmentList).Verifiable();

            NewGroupInCourseRule rule = CreateRule();
            rule.Initialize(new RuleEntry("NewGroup", "\\[ALTA-GRUPO\\]"));

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            this.messageAttachment.Verify(ma => ma.Name, Times.Once());
            this.message.Verify(m => m.Attachments, Times.Exactly(2));
            this.message.Verify(m => m.Subject, Times.Once());
        }

        private NewGroupInCourseRule CreateRule()
        {
            return new NewGroupInCourseRule(this.actionFactory.Object, this.logger.Object);
        }
    }
}
