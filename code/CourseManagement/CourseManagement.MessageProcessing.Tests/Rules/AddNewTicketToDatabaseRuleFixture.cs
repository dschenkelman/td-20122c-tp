using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.MessageProcessing.Rules;
using CourseManagement.Messages;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    [TestClass]
    public class AddNewTicketToDatabaseRuleFixture
    {
        private Mock<IActionFactory> actionFactory;
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> repositories;
        private Mock<IRepository<Ticket>> ticketRepository;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.ticketRepository = this.mockRepository.Create<IRepository<Ticket>>();
            this.repositories.Setup(r => r.Tickets).Returns(this.ticketRepository.Object);
        }

        [TestMethod]
        public void ShouldMatchOnlyIfTicketIsNotInDatabase()
        {
            // arrange
            const string SubjectThatDoesNotMatchPattern = "[CONSULTA-155]";
            const string ValidPublicSubject = "[CONSULTA-PUBLICA] MVC";
            const string ValidPrivateSubject = "[CONSULTA-PRIVADA] patrones de diseño";

            var addTicketReplyToDatabaseRule = this.CreateAddTicketReplyToDatabaseRule();

            Mock<IMessage> message1 = this.mockRepository.Create<IMessage>();
            message1.Setup(m => m.Subject).Returns(SubjectThatDoesNotMatchPattern).Verifiable();

            Mock<IMessage> message2 = this.mockRepository.Create<IMessage>();
            message2.Setup(m => m.Subject).Returns(ValidPublicSubject).Verifiable();

            Mock<IMessage> message3 = this.mockRepository.Create<IMessage>();
            message3.Setup(m => m.Subject).Returns(ValidPrivateSubject).Verifiable();

            this.ticketRepository.Setup(tr => tr.Get(It.Is<Expression<Func<Ticket, bool>>>(f => true))).Returns(
                new List<Ticket>()).Verifiable();

            // act and assert
            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message1.Object, false));
            this.ticketRepository.Verify(tr => tr.Get(It.Is<Expression<Func<Ticket, bool>>>(f => true)), Times.Never());

            Assert.IsTrue(addTicketReplyToDatabaseRule.IsMatch(message2.Object, false));
            this.ticketRepository.Verify(tr => tr.Get(It.Is<Expression<Func<Ticket, bool>>>(f => true)), Times.Once());

            Assert.IsTrue(addTicketReplyToDatabaseRule.IsMatch(message3.Object, false));
            this.ticketRepository.Verify(tr => tr.Get(It.Is<Expression<Func<Ticket, bool>>>(f => true)), Times.Exactly(2));
        }

        [TestMethod]
        public void ShouldNotMatchIfTicketIsInDatabase()
        {
            // arrange
            const string SubjectWithExistingTopic = "[CONSULTA-PUBLICA] contenedor-contenido";

            var addTicketReplyToDatabaseRule = this.CreateAddTicketReplyToDatabaseRule();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns(SubjectWithExistingTopic).Verifiable();

            Ticket ticket = new Ticket {MessageSubject = SubjectWithExistingTopic};
            this.ticketRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Ticket, bool>>>(
                        f =>
                        f.Compile().Invoke(ticket)))).Returns(
                            new List<Ticket>{ ticket }).Verifiable();

            // act and assert

            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message.Object, false));
        }

        private AddNewTicketToDatabaseRule CreateAddTicketReplyToDatabaseRule()
        {
            return new AddNewTicketToDatabaseRule(this.actionFactory.Object, this.repositories.Object);
        }
    }
}
