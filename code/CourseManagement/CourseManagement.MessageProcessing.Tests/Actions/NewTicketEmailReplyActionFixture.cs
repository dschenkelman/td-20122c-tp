using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Model;
using CourseManagement.Persistence.Configuration;
using CourseManagement.Persistence.Logging;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class NewTicketEmailReplyActionFixture
    {
        private Mock<ICourseManagementRepositories> repositories;
        private Mock<IMessageSender> messageSender;
        private Mock<IConfigurationService> configurationService;
        private Mock<IRepository<Ticket>> ticketRepository;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void Initialize()
        {
            this.messageSender = new Mock<IMessageSender>(MockBehavior.Strict);
            this.configurationService = new Mock<IConfigurationService>(MockBehavior.Strict);
            this.repositories = new Mock<ICourseManagementRepositories>(MockBehavior.Strict);
            this.ticketRepository = new Mock<IRepository<Ticket>>(MockBehavior.Strict);

            this.repositories.Setup(r => r.Tickets).Returns(this.ticketRepository.Object);

            this.logger = new Mock<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void ShouldRetrieveSubjectWithIdOfMessageBasedOnDateAndSubject()
        {
            // arrange
            const string Subject = "MessageSubject";
            const int TicketId = 123;
            DateTime creationDate = new DateTime(2012, 11, 5);
            Ticket ticket = new Ticket { MessageSubject = Subject, DateCreated = creationDate, Id = TicketId };

            Mock<IMessage> message = new Mock<IMessage>(MockBehavior.Strict);
            message.Setup(m => m.Date).Returns(creationDate);
            message.Setup(m => m.Subject).Returns(Subject);

            this.ticketRepository
                .Setup(tr => tr.Get(It.Is<Expression<Func<Ticket, bool>>>(e => e.Compile().Invoke(ticket))))
                .Returns(new List<Ticket> { ticket })
                .Verifiable();

            var action = this.CreateNewTicketEmailReplyAction();
            var actionEntry = new ActionEntry("actionName");
            actionEntry.AdditionalData.Add("body", "body");
            actionEntry.AdditionalData.Add("subject", "[Consulta-{0}] Creada");
            actionEntry.AdditionalData.Add("public", "true");

            // act
            action.Initialize(actionEntry);
            string subject = this.InvokeNonPublicMethod<string>(action, "GenerateSubject", message.Object);

            // assert
            Assert.AreEqual("[Consulta-123] Creada", subject);
        }

        public NewTicketEmailReplyAction CreateNewTicketEmailReplyAction()
        {
            return new NewTicketEmailReplyAction(
                this.messageSender.Object,
                this.repositories.Object, 
                this.configurationService.Object,
                this.logger.Object);
        }

        private T InvokeNonPublicMethod<T>(object o, string methodName, params object[] parameters)
        {
            return (T)o.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(o, parameters);
        }
    }
}
