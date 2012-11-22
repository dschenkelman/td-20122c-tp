using System.Collections.Generic;
using System.IO;
using System.IO.Moles;
using System.Linq;
using CourseManagement.Persistence.Configuration;
using CourseManagement.Persistence.Logging;
using CourseManagement.Utilities.Extensions;

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
        private Mock<IConfigurationService> configurationService;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.ticketRepository = this.mockRepository.Create<IRepository<Ticket>>();
            this.repositories.Setup(r => r.Tickets).Returns(this.ticketRepository.Object);

            this.configurationService = this.mockRepository.Create<IConfigurationService>();

            this.logger = this.mockRepository.Create<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<String>()));
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldDownloadAllAttachmentsAndAddThemToDatabase()
        {
            // arrange
            const string AttachmentRootPath = @"C:\";

            const int TicketId = 123;

            this.configurationService.Setup(cs => cs.AttachmentsRootPath).Returns(AttachmentRootPath);

            const string Attachment1Name = "Attachment1";
            const string Attachment2Name = "Attachment2";

            const string Subject = "[Consulta-123]";
            DateTime messageDate = new DateTime(2012, 11, 4);

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns(Subject);
            message.Setup(m => m.Body).Returns("Body");
            message.Setup(m => m.Date).Returns(messageDate);

            string attachmentDirectory = Path.Combine(AttachmentRootPath, Subject, messageDate.ToIsoFormat());
            string attachment1Path = Path.Combine(attachmentDirectory, Attachment1Name);
            string attachment2Path = Path.Combine(attachmentDirectory, Attachment2Name);

            bool directoryCreated = false;
            MDirectory.CreateDirectoryString = p =>
                                                   {
                                                       directoryCreated = true;
                                                       Assert.AreEqual(attachmentDirectory, p);
                                                       return null;
                                                   };

            Mock<IMessageAttachment> attachment1 = this.mockRepository.Create<IMessageAttachment>();
            attachment1.Setup(a => a.Name).Returns(Attachment1Name);
            attachment1.Setup(a => a.Download(attachment1Path)).Verifiable();

            Mock<IMessageAttachment> attachment2 = this.mockRepository.Create<IMessageAttachment>();
            attachment2.Setup(a => a.Name).Returns(Attachment2Name);
            attachment2.Setup(a => a.Download(attachment2Path)).Verifiable();

            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment>()
                                                          {
                                                             attachment1.Object, attachment2.Object
                                                          });

            Ticket ticket = new Ticket { Id = TicketId };

            this.ticketRepository.Setup(tr => tr.GetById(123)).Returns(ticket);

            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            var relateTicketReplyToTicketAction = this.CreateRelateTicketReplyToTicketAction();

            // act
            relateTicketReplyToTicketAction.Execute(message.Object);

            // assert
            Assert.IsTrue(directoryCreated);
            attachment1.Verify(a => a.Download(attachment1Path), Times.Once());
            attachment2.Verify(a => a.Download(attachment2Path), Times.Once());

            Assert.AreEqual(2, ticket.Attachments.Count);
            var ticketAttachment1 = ticket.Attachments.ElementAt(0);
            var ticketAttachment2 = ticket.Attachments.ElementAt(1);

            Assert.AreEqual(Attachment1Name, ticketAttachment1.FileName);
            Assert.AreEqual(attachment1Path, ticketAttachment1.Location);

            Assert.AreEqual(Attachment2Name, ticketAttachment2.FileName);
            Assert.AreEqual(attachment2Path, ticketAttachment2.Location);

            this.ticketRepository.Verify(tr => tr.GetById(123), Times.Once());
            this.ticketRepository.Verify(tr => tr.Save(), Times.Once());
        }

        public DownloadReplyAttachmentsAction CreateRelateTicketReplyToTicketAction()
        {
            return new DownloadReplyAttachmentsAction(
                this.repositories.Object,
                this.configurationService.Object, this.logger.Object);
        }
    }
}