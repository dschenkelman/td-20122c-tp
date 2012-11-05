namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System;
    using MessageProcessing.Actions;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class DownloadReplyAttachmentsActionFixture
    {
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> repositories;
        private Mock<IRepository<Ticket>> ticketRepository;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.ticketRepository = this.mockRepository.Create<IRepository<Ticket>>();
            this.repositories.Setup(r => r.Tickets).Returns(this.ticketRepository.Object);
        }

        [TestMethod]
        public void ShouldDownloadAllAttachmentsAndAddThemToDatabase()
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
            relateTicketReplyToTicketAction.Execute(message.Object);

            // assert
        }

        public DownloadReplyAttachmentsAction CreateRelateTicketReplyToTicketAction()
        {
            return new DownloadReplyAttachmentsAction(this.repositories.Object);
        }
    }
}
