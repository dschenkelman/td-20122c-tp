using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CourseManagement.MessageProcessing.Services;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class AddTicketReplyToDatabaseRuleFixture
    {
        private Mock<IActionFactory> actionFactory;
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> repositories;
        private Mock<IRepository<Ticket>> ticketRepository;
        private Mock<IConfigurationService> configurationService;
        private Mock<IRepository<Course>> courseRepository;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.ticketRepository = this.mockRepository.Create<IRepository<Ticket>>();
            this.configurationService = this.mockRepository.Create<IConfigurationService>();
            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.repositories.Setup(r => r.Tickets).Returns(this.ticketRepository.Object);
            this.repositories.Setup(r => r.Courses).Returns(this.courseRepository.Object);
        }

        [TestMethod]
        public void ShouldMatchOnlyIfTicketIdIsInDatabase()
        {
            // arrange
            const int SubjectId = 1;
            const string SubjectThatDoesNotMatchPattern = "[CONSULTA-Privada]";
            const string SubjectWithNonExistentTicketId = "[CONSULTA-10]";
            const string SubjectWithNonIntegerTicketId = "[CONSULTA-999999999999999999]";
            const string ValidSubject = "[CONSULTA-1]";
            const string FromAddress = "From@From.com";
            DateTime dateTime = new DateTime(2012, 11, 5);

            var addTicketReplyToDatabaseRule = this.CreateAddTicketReplyToDatabaseRule();
            addTicketReplyToDatabaseRule.Initialize(new RuleEntry("TicketReply", "^\\[CONSULTA-(?<ticketId>[0-9]*)\\]$"));

            Mock<IMessage> message1 = this.mockRepository.Create<IMessage>();
            message1.Setup(m => m.Subject).Returns(SubjectThatDoesNotMatchPattern);
            message1.Setup(m => m.From).Returns(FromAddress);
            message1.Setup(m => m.Date).Returns(dateTime);

            Mock<IMessage> message2 = this.mockRepository.Create<IMessage>();
            message2.Setup(m => m.Subject).Returns(SubjectWithNonExistentTicketId);
            message2.Setup(m => m.From).Returns(FromAddress);
            message2.Setup(m => m.Date).Returns(dateTime);

            Mock<IMessage> message3 = this.mockRepository.Create<IMessage>();
            message3.Setup(m => m.Subject).Returns(ValidSubject);
            message3.Setup(m => m.From).Returns(FromAddress);
            message3.Setup(m => m.Date).Returns(dateTime);

            Mock<IMessage> message4 = this.mockRepository.Create<IMessage>();
            message4.Setup(m => m.Subject).Returns(SubjectWithNonIntegerTicketId);
            message4.Setup(m => m.From).Returns(FromAddress);
            message4.Setup(m => m.Date).Returns(dateTime);

            Ticket ticket = new Ticket();
            this.ticketRepository.Setup(tr => tr.GetById(10)).Returns(default(Ticket)).Verifiable();
            this.ticketRepository.Setup(tr => tr.GetById(1)).Returns(ticket).Verifiable();

            Course course = new Course(2, 2012, SubjectId);
            course.Students.Add(new Student { MessagingSystemId = FromAddress });

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId);
            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns(new List<Course> { course })
                .Verifiable();

            // act and assert
            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message1.Object, false));

            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message2.Object, false));
            this.ticketRepository.Verify(tr => tr.GetById(10), Times.Once());

            Assert.IsTrue(addTicketReplyToDatabaseRule.IsMatch(message3.Object, false));
            this.ticketRepository.Verify(tr => tr.GetById(1), Times.Once());

            Assert.IsFalse(addTicketReplyToDatabaseRule.IsMatch(message4.Object, false));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowIfFromIsNotPartOfCourseEitherAsStudentOrTeacher()
        {
            // arrange
            const string FromAddress = "From@From.com";
            const int SubjectId = 1;
            const string MessageSubject = "[CONSULTA-1]";
            DateTime messageDate = new DateTime(2012, 11, 4);

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns(MessageSubject);
            message.Setup(m => m.Date).Returns(messageDate);
            message.Setup(m => m.From).Returns(FromAddress);

            Course course = new Course(2, 2012, SubjectId);
            Course notMatchingCourseBecauseOfSemester = new Course(1, 2012, SubjectId);
            Course notMatchingCourseBecauseOfYear = new Course(2, 2011, SubjectId);
            Course notMatchingCourseBecauseOfSubject = new Course(1, 2012, 0);

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId);
            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                e => e.Compile().Invoke(course)
                    && !e.Compile().Invoke(notMatchingCourseBecauseOfSemester)
                    && !e.Compile().Invoke(notMatchingCourseBecauseOfYear)
                    && !e.Compile().Invoke(notMatchingCourseBecauseOfSubject))))
                .Returns(new List<Course> { course })
                .Verifiable();

            var addTicketReplyToDatabaseRule = this.CreateAddTicketReplyToDatabaseRule();
            addTicketReplyToDatabaseRule.Initialize(new RuleEntry("TicketReply", "^\\[CONSULTA-(?<ticketId>[0-9]*)\\]$"));

            Ticket ticket = new Ticket();
            this.ticketRepository.Setup(tr => tr.GetById(1)).Returns(ticket).Verifiable();

            addTicketReplyToDatabaseRule.IsMatch(message.Object, false);
        }

        public AddTicketReplyToDatabaseRule CreateAddTicketReplyToDatabaseRule()
        {
            return new AddTicketReplyToDatabaseRule(
                this.actionFactory.Object,
                this.repositories.Object,
                this.configurationService.Object);
        }
    }
}
