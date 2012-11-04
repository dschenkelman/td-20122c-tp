namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using MessageProcessing.Actions;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;
    using CourseManagement.MessageProcessing.Rules;

    [TestClass]
    public class AddTicketReplyToDatabaseRuleFixture
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
        public void ShouldMatchOnlyIfTicketIdIsInDatabase()
        {
            // arrange
            const string SubjectThatDoesNotMatchPattern = "[Consulta-Privada]";
            const string SubjectWithNonExistentTicketId = "[Consulta-10]";
            const string SubjectWithNonIntegerTicketId = "[Consulta-999999999999999999]";
            const string ValidSubject = "[Consulta-1]";

            var addTicketReplyToDatabaseRule = this.CreateAddTicketReplyToDatabaseRule();

            Mock<IMessage> message1 = this.mockRepository.Create<IMessage>();
            message1.Setup(m => m.Subject).Returns(SubjectThatDoesNotMatchPattern).Verifiable();

            Mock<IMessage> message2 = this.mockRepository.Create<IMessage>();
            message2.Setup(m => m.Subject).Returns(SubjectWithNonExistentTicketId).Verifiable();

            Mock<IMessage> message3 = this.mockRepository.Create<IMessage>();
            message3.Setup(m => m.Subject).Returns(ValidSubject).Verifiable();

            Mock<IMessage> message4 = this.mockRepository.Create<IMessage>();
            message4.Setup(m => m.Subject).Returns(SubjectWithNonIntegerTicketId).Verifiable();

            Ticket ticket = new Ticket();
            this.ticketRepository.Setup(tr => tr.GetById(10)).Returns(default(Ticket)).Verifiable();
            this.ticketRepository.Setup(tr => tr.GetById(1)).Returns(ticket).Verifiable();

            // act and assert
            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message1.Object, false));

            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message2.Object, false));
            this.ticketRepository.Verify(tr => tr.GetById(10), Times.Once());

            Assert.IsTrue(addTicketReplyToDatabaseRule.IsMatch(message3.Object, false));
            this.ticketRepository.Verify(tr => tr.GetById(1), Times.Once());

            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message4.Object, false));
        }

        public AddTicketReplyToDatabaseRule CreateAddTicketReplyToDatabaseRule()
        {
            return new AddTicketReplyToDatabaseRule(this.actionFactory.Object, this.repositories.Object);
        }
    }
}
