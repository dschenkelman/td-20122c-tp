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
    public class CreateReplyEmailActionFixture
    {
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<IConfigurationService> configurationService;
        private Mock<IMessageSender> messageSender;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();

            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);

            this.configurationService = this.mockRepository.Create<IConfigurationService>();

            this.messageSender = this.mockRepository.Create<IMessageSender>();
        }

        [TestMethod]
        public void ShouldSendMessage()
        {
            // arrange
            const string Protocol = "stmp";
            Configuration configuration = new Configuration { Protocol = Protocol, Endpoint = "end", Port = 25, UseSsl = true };

            Account account = new Account
            {
                User = "tecnicas.de.diseño@yahoo.com.ar",
                Password = "123456",
                Configurations = new List<Configuration> { configuration }
            };

            configuration.Account = account;

            const int Semester = 2;
            const int Year = 2012;
            const int SubjectId = 7510;
            Course trueCourse = new Course(Semester, Year, SubjectId) { Account = account };
            Course falseCourseWrongYear = new Course(Semester, 2010, SubjectId);
            Course falseCourseWrongSubjectId = new Course(Semester, Year, 7515);
            Course falseCourseWrongSemester = new Course(3, Year, SubjectId);

            List<Course> courses = new List<Course> { trueCourse };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                f =>
                f.Compile().Invoke(trueCourse) && !f.Compile().Invoke(falseCourseWrongYear) &&
                !f.Compile().Invoke(falseCourseWrongSubjectId) && !f.Compile().Invoke(falseCourseWrongSemester)))).
                Returns(courses).Verifiable();

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId).Verifiable();
            this.configurationService.Setup(cs => cs.OutgoingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageSender.Setup(ms => ms.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            this.messageSender.Setup(ms => ms.Send(It.IsAny<IMessage>())).Verifiable();
            this.messageSender.Setup(ms => ms.Disconnect()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.From).Returns("matias.servetto@gmail.com");
            message.Setup(m => m.Subject).Returns("[CNSOULTA-PRIVADA] tema1");

            CreateEmailReplyAction action = CreateAction();

            ActionEntry actionEntry = new ActionEntry("CONSULTA");
            actionEntry.AdditionalData.Add("public", "true");
            actionEntry.AdditionalData.Add("body", "body text");

            // act
            action.Initialize(actionEntry);
            action.Execute(message.Object);

            //validate
            this.messageSender.Verify(ms => ms.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            this.messageSender.Verify(ms => ms.Send(It.IsAny<IMessage>()), Times.Once());
            this.messageSender.Verify(ms => ms.Disconnect(), Times.Once());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotSendMessageIfNoConfigurationIsRetrieved()
        {
            // arrange
            const string Protocol = "stmp";
            Configuration configuration = new Configuration { Protocol = Protocol, Endpoint = "end", Port = 25, UseSsl = true };

            Account account = new Account
            {
                User = "tecnicas.de.diseño@yahoo.com.ar",
                Password = "123456",
                Configurations = new List<Configuration> {}
            };

            configuration.Account = account;

            const int Semester = 2;
            const int Year = 2012;
            const int SubjectId = 7510;
            Course trueCourse = new Course(Semester, Year, SubjectId) { Account = account };
            Course falseCourseWrongYear = new Course(Semester, 2010, SubjectId);
            Course falseCourseWrongSubjectId = new Course(Semester, Year, 7515);
            Course falseCourseWrongSemester = new Course(3, Year, SubjectId);

            List<Course> courses = new List<Course> { trueCourse };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                f =>
                f.Compile().Invoke(trueCourse) && !f.Compile().Invoke(falseCourseWrongYear) &&
                !f.Compile().Invoke(falseCourseWrongSubjectId) && !f.Compile().Invoke(falseCourseWrongSemester)))).
                Returns(courses).Verifiable();

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId).Verifiable();
            this.configurationService.Setup(cs => cs.OutgoingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageSender.Setup(
                ms =>
                ms.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
            this.messageSender.Setup(ms => ms.Send(It.IsAny<IMessage>())).Verifiable();
            this.messageSender.Setup(ms => ms.Disconnect()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.From).Returns("matias.servetto@gmail.com");
            message.Setup(m => m.Subject).Returns("[CNSOULTA-PRIVADA] tema1");

            CreateEmailReplyAction action = CreateAction();

            ActionEntry actionEntry = new ActionEntry("CONSULTA");
            actionEntry.AdditionalData.Add("public", "true");
            actionEntry.AdditionalData.Add("body", "body text");

            // act
            action.Initialize(actionEntry);
            action.Execute(message.Object);

            //validate
            this.messageSender.Verify(ms => ms.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            this.messageSender.Verify(ms => ms.Send(It.IsAny<IMessage>()), Times.Never());
            this.messageSender.Verify(ms => ms.Disconnect(), Times.Never());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotSendMessageIfCourseIsNotRetrieved()
        {
            // arrange
            const string Protocol = "stmp";
            Configuration configuration = new Configuration { Protocol = Protocol, Endpoint = "end", Port = 25, UseSsl = true };

            Account account = new Account
            {
                User = "tecnicas.de.diseño@yahoo.com.ar",
                Password = "123456",
                Configurations = new List<Configuration> { configuration }
            };

            configuration.Account = account;

            const int Semester = 2;
            const int Year = 2012;
            const int SubjectId = 7510;
            Course trueCourse = new Course(Semester, Year, SubjectId) { Account = account };
            Course falseCourseWrongYear = new Course(Semester, 2010, SubjectId);
            Course falseCourseWrongSubjectId = new Course(Semester, Year, 7515);
            Course falseCourseWrongSemester = new Course(3, Year, SubjectId);

            List<Course> courses = new List<Course> {};

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                f =>
                f.Compile().Invoke(trueCourse) && !f.Compile().Invoke(falseCourseWrongYear) &&
                !f.Compile().Invoke(falseCourseWrongSubjectId) && !f.Compile().Invoke(falseCourseWrongSemester)))).
                Returns(courses).Verifiable();

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId).Verifiable();
            this.configurationService.Setup(cs => cs.OutgoingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageSender.Setup(
                ms =>
                ms.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
            this.messageSender.Setup(ms => ms.Send(It.IsAny<IMessage>())).Verifiable();
            this.messageSender.Setup(ms => ms.Disconnect()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.From).Returns("matias.servetto@gmail.com");
            message.Setup(m => m.Subject).Returns("[CNSOULTA-PRIVADA] tema1");

            CreateEmailReplyAction action = CreateAction();

            ActionEntry actionEntry = new ActionEntry("CONSULTA");
            actionEntry.AdditionalData.Add("public", "true");
            actionEntry.AdditionalData.Add("body", "body text");

            // act
            action.Initialize(actionEntry);
            action.Execute(message.Object);

            //validate
            this.messageSender.Verify(ms => ms.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            this.messageSender.Verify(ms => ms.Send(It.IsAny<IMessage>()), Times.Never());
            this.messageSender.Verify(ms => ms.Disconnect(), Times.Never());
        }

        private CreateEmailReplyAction CreateAction()
        {
            return new CreateEmailReplyAction(this.messageSender.Object, this.courseManagementRepositories.Object, this.configurationService.Object);
        }
    }
}
