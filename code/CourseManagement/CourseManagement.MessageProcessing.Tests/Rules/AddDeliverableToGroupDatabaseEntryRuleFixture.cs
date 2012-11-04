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
    public class AddDeliverableToGroupDatabaseEntryRuleFixture
    {
        private MockRepository mockRepository;
        private Mock<IActionFactory> actionFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
        }

        [TestMethod]
        public void ShouldMatchMessage()
        {
            // arrange
            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ENTREGA-TP-1]");
            message.Setup(m => m.From).Returns("servetto.matias@gmail.com");
            Mock<IMessageAttachment> messageAttachment = this.mockRepository.Create<IMessageAttachment>();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment> {messageAttachment.Object}).Verifiable();

            AddDeliverableToGroupDatabaseEntryRule rule = CreateRule();

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsTrue(resultado);
            message.Verify(m => m.Attachments, Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithWrongSubject()
        {
            // arrange
            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[TP-5]");
            message.Setup(m => m.From).Returns("servetto.matias@gmail.com");
            Mock<IMessageAttachment> messageAttachment = this.mockRepository.Create<IMessageAttachment>();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment> { messageAttachment.Object }).Verifiable();

            AddDeliverableToGroupDatabaseEntryRule rule = CreateRule();

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            message.Verify(m => m.Attachments, Times.Never());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithoutAttachments()
        {
            // arrange
            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ENTREGA-TP-5]");
            message.Setup(m => m.From).Returns("sebastianr312@gmail.com");
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment>()).Verifiable();

            AddDeliverableToGroupDatabaseEntryRule rule = CreateRule();

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsFalse(resultado);
            message.Verify(m => m.Attachments, Times.Once());
        }

        private AddDeliverableToGroupDatabaseEntryRule CreateRule()
        {
            return new AddDeliverableToGroupDatabaseEntryRule(this.actionFactory.Object);
        }
    }
}
