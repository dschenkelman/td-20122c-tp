namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MessageProcessing.Actions;
    using MessageProcessing.Services;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class UpdateTicketStatusActionFixture
    {
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> repositories;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<IRepository<Ticket>> ticketRepository;
        private Mock<IConfigurationService> configurationService;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.configurationService = this.mockRepository.Create<IConfigurationService>();

            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.ticketRepository = this.mockRepository.Create<IRepository<Ticket>>();

            this.repositories.Setup(r => r.Courses).Returns(this.courseRepository.Object);
            this.repositories.Setup(r => r.Tickets).Returns(this.ticketRepository.Object);
        }

        [TestMethod]
        public void ShouldMoveTicketToPendingWhenItIsAnsweredByTeacher()
        {
            // arrange
            const string FromAddress = "From@From.com";
            const int TicketId = 123;

            const int SubjectId = 7510;
            const string MessageSubject = "[Consulta-123]";
            DateTime messageDate = new DateTime(2012, 11, 4);

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns(MessageSubject);
            message.Setup(m => m.Date).Returns(messageDate);
            message.Setup(m => m.From).Returns(FromAddress);

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId);

            Course course = new Course(2, 2012, SubjectId);
            Course notMatchingCourseBecauseOfSemester = new Course(1, 2012, SubjectId);
            Course notMatchingCourseBecauseOfYear = new Course(2, 2011, SubjectId);
            Course notMatchingCourseBecauseOfSubject = new Course(1, 2012, 0);

            Teacher teacher = new Teacher(1, "Not important", FromAddress);
            Teacher notMatchingTeacherBecauseOfAddress = new Teacher(1, "Not important", "Another address");
            course.Teachers.Add(teacher);
            course.Teachers.Add(notMatchingTeacherBecauseOfAddress);

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                e => e.Compile().Invoke(course)
                    && !e.Compile().Invoke(notMatchingCourseBecauseOfSemester)
                    && !e.Compile().Invoke(notMatchingCourseBecauseOfYear)
                    && !e.Compile().Invoke(notMatchingCourseBecauseOfSubject))))
                .Returns(new List<Course> { course })
                .Verifiable();

            Ticket ticket = new Ticket { State = TicketState.Assigned, Id = TicketId };
            this.ticketRepository.Setup(tr => tr.GetById(TicketId)).Returns(ticket).Verifiable();
            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            var updateTicketStatusAction = this.CreateUpdateTicketStatusAction();

            // act
            updateTicketStatusAction.Execute(message.Object);

            // assert
            Assert.AreEqual(TicketState.Pending, ticket.State);
            Assert.AreEqual(teacher.Id, ticket.TeacherId);
            this.ticketRepository.Verify(tr => tr.GetById(TicketId), Times.Once());
            this.ticketRepository.Verify(tr => tr.Save(), Times.Once());
            this.courseRepository.Verify(
                cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                e => e.Compile().Invoke(course)
                     && !e.Compile().Invoke(notMatchingCourseBecauseOfSemester)
                     && !e.Compile().Invoke(notMatchingCourseBecauseOfYear)
                     && !e.Compile().Invoke(notMatchingCourseBecauseOfSubject))), 
                     Times.Once());
        }

        [TestMethod]
        public void ShouldMoveTicketToAssignedWhenItIsPendingAndAnsweredByStudentAndAlreadyHasAssignedTeacher()
        {
            // arrange
            const string FromAddress = "From@From.com";
            const int TicketId = 123;

            const int SubjectId = 7510;
            const string MessageSubject = "[Consulta-123]";
            DateTime messageDate = new DateTime(2012, 11, 4);

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns(MessageSubject);
            message.Setup(m => m.Date).Returns(messageDate);
            message.Setup(m => m.From).Returns(FromAddress);

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId);

            Course course = new Course(2, 2012, SubjectId);

            Teacher teacher = new Teacher(1, "Not important", "Another address");
            course.Teachers.Add(teacher);

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns(new List<Course> { course })
                .Verifiable();

            Ticket ticket = new Ticket { State = TicketState.Pending, Id = TicketId, TeacherId = teacher.Id };
            this.ticketRepository.Setup(tr => tr.GetById(TicketId)).Returns(ticket).Verifiable();
            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            var updateTicketStatusAction = this.CreateUpdateTicketStatusAction();

            // act
            updateTicketStatusAction.Execute(message.Object);

            // assert
            Assert.AreEqual(TicketState.Assigned, ticket.State);
            Assert.AreEqual(teacher.Id, ticket.TeacherId);
            this.ticketRepository.Verify(tr => tr.GetById(TicketId), Times.Once());
            this.ticketRepository.Verify(tr => tr.Save(), Times.Once());
        }

        private UpdateTicketStatusAction CreateUpdateTicketStatusAction()
        {
            return new UpdateTicketStatusAction(this.configurationService.Object, this.repositories.Object);
        }
    }
}
