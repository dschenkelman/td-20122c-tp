namespace CourseManagement.MessageProcessing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MessageProcessing.Rules;
    using MessageProcessing.Services;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class MessageProcessorFixture
    {
        private MockRepository mockRepository;
        private Mock<IRuleFactory> ruleFactory;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<IConfigurationService> configurationService;
        private Mock<IMessageReceiver> messageReceiver;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.ruleFactory = this.mockRepository.Create<IRuleFactory>();
            
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();

            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);

            this.configurationService = this.mockRepository.Create<IConfigurationService>();
            this.messageReceiver = this.mockRepository.Create<IMessageReceiver>();
        }

        [TestMethod]
        public void ShouldRetrieveRulesFromRuleFactoryWhenProcessingMessages()
        {
            // arrange
            const string Protocol = "pop";
            Configuration configuration = new Configuration
                                              {Protocol = Protocol, Endpoint = "end", Port = 25, UseSsl = true};

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

            List<Course> courses = new List<Course> {trueCourse};

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                f =>
                f.Compile().Invoke(trueCourse) && !f.Compile().Invoke(falseCourseWrongYear) &&
                !f.Compile().Invoke(falseCourseWrongSubjectId) && !f.Compile().Invoke(falseCourseWrongSemester)))).
                Returns(courses).Verifiable();

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId).Verifiable();
            this.configurationService.Setup(cs => cs.IncomingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageReceiver.Setup(
                mr => mr.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl,
                                 configuration.Account.User, configuration.Account.Password)).Verifiable();

            this.messageReceiver.Setup(mr => mr.FetchMessages()).Returns(new List<IMessage>()).Verifiable();

            this.messageReceiver.Setup(mr => mr.Disconnect()).Verifiable();

            this.ruleFactory.Setup(rf => rf.CreateRules()).Returns(new List<BaseRule>()).Verifiable();

            MessageProcessor messageProcessor = this.CreateMessageProcessor();

            // act
            messageProcessor.Process();

            // assert
            this.ruleFactory.Verify(rf => rf.CreateRules(), Times.Once());
        }

        [TestMethod]
        public void ShouldConnectAndDisconnectIncomingConnectionFromMessageReceiver()
        {
            // arrange
            const string Protocol = "pop";
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
            this.configurationService.Setup(cs => cs.IncomingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageReceiver.Setup(
                mr => mr.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl,
                                 configuration.Account.User, configuration.Account.Password)).Verifiable();

            this.messageReceiver.Setup(mr => mr.FetchMessages()).Returns(new List<IMessage>()).Verifiable();

            this.messageReceiver.Setup(mr => mr.Disconnect()).Verifiable();

            this.ruleFactory.Setup(rf => rf.CreateRules()).Returns(new List<BaseRule>()).Verifiable();

            MessageProcessor messageProcessor = this.CreateMessageProcessor();

            // act
            messageProcessor.Process();

            // assert
            this.messageReceiver.Verify(
                mr => mr.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl,
                                 configuration.Account.User, configuration.Account.Password), Times.Once());

            this.messageReceiver.Verify(mr => mr.FetchMessages(), Times.Once());

            this.messageReceiver.Verify(mr => mr.Disconnect(), Times.Once());
        }

        [TestMethod]
        public void ShouldNotMekeAConnectionBecauseCantFindCourseAtDatabaseRepository()
        {
            // arrange
            const string Protocol = "pop";
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
            this.configurationService.Setup(cs => cs.IncomingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageReceiver.Setup(
                mr => mr.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl,
                                 configuration.Account.User, configuration.Account.Password)).Verifiable();

            this.messageReceiver.Setup(mr => mr.FetchMessages()).Returns(new List<IMessage>()).Verifiable();

            this.messageReceiver.Setup(mr => mr.Disconnect()).Verifiable();

            this.ruleFactory.Setup(rf => rf.CreateRules()).Returns(new List<BaseRule>()).Verifiable();

            MessageProcessor messageProcessor = this.CreateMessageProcessor();

            // act
            messageProcessor.Process();

            // assert
            this.messageReceiver.Verify(
                mr => mr.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl,
                                 configuration.Account.User, configuration.Account.Password), Times.Never());

            this.messageReceiver.Verify(mr => mr.FetchMessages(), Times.Never());

            this.messageReceiver.Verify(mr => mr.Disconnect(), Times.Never());
        }

        private MessageProcessor CreateMessageProcessor()
        {
            return new MessageProcessor(this.ruleFactory.Object, this.courseManagementRepositories.Object,
                                        this.configurationService.Object, this.messageReceiver.Object);
        }
    }
}
