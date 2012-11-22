using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System;
    using System.Linq;
    using MessageProcessing.Actions;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class RelateTicketReplyToTicketActionFixture
    {
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> repositories;
        private Mock<IRepository<Ticket>> ticketRepository;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.ticketRepository = this.mockRepository.Create<IRepository<Ticket>>();
            this.repositories.Setup(r => r.Tickets).Returns(this.ticketRepository.Object);
            this.logger = this.mockRepository.Create<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<String>()));
        }

        [TestMethod]
        public void ShouldAddReplyToTicketGettingPropertiesFromMessage()
        {
            // arrange
            const int TicketId = 123;
            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[Consulta-123]");
            message.Setup(m => m.Body).Returns("Body");
            message.Setup(m => m.Date).Returns(new DateTime(2012, 11, 4));
            
            Ticket ticket = new Ticket { Id = TicketId };
            
            this.ticketRepository.Setup(tr => tr.GetById(123)).Returns(ticket);

            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            var relateTicketReplyToTicketAction = this.CreateRelateTicketReplyToTicketAction();

            // act
            relateTicketReplyToTicketAction.Execute(message.Object, this.logger.Object);

            // assert
            Assert.AreEqual(1, ticket.Replies.Count);
            
            var reply = ticket.Replies.ElementAt(0);
            Assert.AreEqual("[Consulta-123]", reply.MessageSubject);
            Assert.AreEqual("Body", reply.MessageBody);
            Assert.AreEqual(new DateTime(2012, 11, 4), reply.DateCreated);

            this.ticketRepository.Verify(tr => tr.Save(), Times.Once());
        }

        public RelateTicketReplyToTicketAction CreateRelateTicketReplyToTicketAction()
        {
            return new RelateTicketReplyToTicketAction(this.repositories.Object);
        }
    }
}
