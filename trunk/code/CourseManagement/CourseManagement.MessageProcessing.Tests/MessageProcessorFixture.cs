using CourseManagement.MessageProcessing.Actions;
using CourseManagement.MessageProcessing.Rules.Moles;
using CourseManagement.Persistence.Configuration;

namespace CourseManagement.MessageProcessing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MessageProcessing.Rules;
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
                                              { Protocol = Protocol, Endpoint = "end", Port = 25, UseSsl = true };

            Account account = new Account
                                  {
                                      User = "tecnicas.de.diseño@yahoo.com.ar",
                                      Password = "123456",
                                  };

            account.Configurations.Add(configuration);

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
                f.Compile().Invoke(trueCourse) 
                && !f.Compile().Invoke(falseCourseWrongYear) 
                && !f.Compile().Invoke(falseCourseWrongSubjectId) 
                && !f.Compile().Invoke(falseCourseWrongSemester))))
                .Returns(courses).Verifiable();

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId).Verifiable();
            this.configurationService.Setup(cs => cs.IncomingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageReceiver.Setup(
                mr => mr.Connect(
                    configuration.Endpoint,
                    configuration.Port,
                    configuration.UseSsl,
                    configuration.Account.User,
                    configuration.Account.Password)).Verifiable();

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
            };

            account.Configurations.Add(configuration);

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
                !f.Compile().Invoke(falseCourseWrongSubjectId) &&
                !f.Compile().Invoke(falseCourseWrongSemester))))
                .Returns(courses).Verifiable();

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId).Verifiable();
            this.configurationService.Setup(cs => cs.IncomingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageReceiver.Setup(
                mr => mr.Connect(
                    configuration.Endpoint,
                    configuration.Port, 
                    configuration.UseSsl,
                    configuration.Account.User, 
                    configuration.Account.Password)).Verifiable();

            this.messageReceiver.Setup(mr => mr.FetchMessages()).Returns(new List<IMessage>()).Verifiable();

            this.messageReceiver.Setup(mr => mr.Disconnect()).Verifiable();

            this.ruleFactory.Setup(rf => rf.CreateRules()).Returns(new List<BaseRule>()).Verifiable();

            MessageProcessor messageProcessor = this.CreateMessageProcessor();

            // act
            messageProcessor.Process();

            // assert
            this.messageReceiver.Verify(
                mr => mr.Connect(
                    configuration.Endpoint, 
                    configuration.Port, 
                    configuration.UseSsl,
                    configuration.Account.User,
                    configuration.Account.Password),
                    Times.Once());

            this.messageReceiver.Verify(mr => mr.FetchMessages(), Times.Once());

            this.messageReceiver.Verify(mr => mr.Disconnect(), Times.Once());
        }

        [TestMethod]
        public void ShouldNotMakeAConnectionBecauseCantFindCourseAtDatabaseRepository()
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

            account.Configurations.Add(configuration);

            configuration.Account = account;

            const int Semester = 2;
            const int Year = 2012;
            const int SubjectId = 7510;
            Course trueCourse = new Course(Semester, Year, SubjectId) { Account = account };
            Course falseCourseWrongYear = new Course(Semester, 2010, SubjectId);
            Course falseCourseWrongSubjectId = new Course(Semester, Year, 7515);
            Course falseCourseWrongSemester = new Course(3, Year, SubjectId);

            List<Course> courses = new List<Course>();

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                f =>
                f.Compile().Invoke(trueCourse) 
                && !f.Compile().Invoke(falseCourseWrongYear)
                && !f.Compile().Invoke(falseCourseWrongSubjectId) 
                && !f.Compile().Invoke(falseCourseWrongSemester))))
                .Returns(courses).Verifiable();

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId).Verifiable();
            this.configurationService.Setup(cs => cs.IncomingMessageProtocol).Returns(Protocol).Verifiable();

            this.messageReceiver.Setup(
                mr => mr.Connect(
                    configuration.Endpoint,
                    configuration.Port,
                    configuration.UseSsl,
                    configuration.Account.User,
                    configuration.Account.Password)).Verifiable();

            this.messageReceiver.Setup(mr => mr.FetchMessages()).Returns(new List<IMessage>()).Verifiable();

            this.messageReceiver.Setup(mr => mr.Disconnect()).Verifiable();

            this.ruleFactory.Setup(rf => rf.CreateRules()).Returns(new List<BaseRule>()).Verifiable();

            MessageProcessor messageProcessor = this.CreateMessageProcessor();

            // act
            messageProcessor.Process();

            // assert
            this.messageReceiver.Verify(
                mr => mr.Connect(
                    configuration.Endpoint, 
                    configuration.Port,
                    configuration.UseSsl,
                    configuration.Account.User,
                    configuration.Account.Password),
                    Times.Never());

            this.messageReceiver.Verify(mr => mr.FetchMessages(), Times.Never());

            this.messageReceiver.Verify(mr => mr.Disconnect(), Times.Never());
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldInvokeProcessForRuleThatMatchPassingMessageAndBooleanOnWhetherARuleHasMatched()
        {
            // arrange
            const string Protocol = "pop";
            Configuration configuration = new Configuration
                                              {
                                                  Protocol = Protocol,
                                                  Endpoint = "end",
                                                  Port = 25,
                                                  UseSsl = true
                                              };

            Account account = new Account
                                  {
                                      User = "tecnicas.de.diseño@yahoo.com.ar",
                                      Password = "123456",
                                  };
            account.Configurations.Add(configuration);

            configuration.Account = account;

            const int SubjectId = 7510;
            const int Semester = 2;
            const int Year = 2012;

            Course course = new Course(Semester, Year, SubjectId) { Account = account };

            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns(courses);

            this.configurationService.Setup(cs => cs.MonitoredSubjectId).Returns(SubjectId);
            this.configurationService.Setup(cs => cs.IncomingMessageProtocol).Returns(Protocol);

            this.messageReceiver.Setup(
                mr => mr.Connect(
                    It.IsAny<string>(), 
                    It.IsAny<int>(), 
                    It.IsAny<bool>(), 
                    It.IsAny<string>(),
                    It.IsAny<string>())).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();

            var mockActionFactory = this.mockRepository.Create<IActionFactory>();

            this.messageReceiver.Setup(mr => mr.FetchMessages())
                .Returns(new List<IMessage> { message.Object });

            this.messageReceiver.Setup(mr => mr.Disconnect());

            bool rule1IsMatchInvoked = false;
            bool rule2IsMatchInvoked = true;
            bool rule3IsMatchInvoked = true;
            bool rule1ProcessInvoked = false;
            bool rule2ProcessInvoked = true;
            bool rule3ProcessInvoked = true;

            SBaseRule rule1 = new SBaseRule(mockActionFactory.Object);
            rule1.IsMatchIMessageBoolean = (m, pm) =>
                                               {
                                                    rule1IsMatchInvoked = true;
                                                    Assert.IsFalse(pm);
                                                    return false;
                                                };

            SBaseRule rule2 = new SBaseRule(mockActionFactory.Object);
            rule2.IsMatchIMessageBoolean = (m, pm) =>
                                                {
                                                    rule2IsMatchInvoked = true;
                                                    Assert.IsFalse(pm);
                                                    return true;
                                                };

            SBaseRule rule3 = new SBaseRule(mockActionFactory.Object);
            rule3.IsMatchIMessageBoolean = (m, pm) =>
                                                   {
                                                       rule3IsMatchInvoked = true;
                                                       Assert.IsTrue(pm);
                                                       return true;
                                                   };

            MBaseRule.AllInstances.ProcessIMessage = (r, m) =>
            {
                if (rule1 == r)
                {
                    rule1ProcessInvoked = true;
                }

                if (rule2 == r)
                {
                    rule2ProcessInvoked = true;
                }

                if (rule3 == r)
                {
                    rule3ProcessInvoked = true;
                }
            };

            var rules = new List<BaseRule> { rule1, rule2, rule3 };

            this.ruleFactory
                .Setup(rf => rf.CreateRules())
                .Returns(rules).Verifiable();

            MessageProcessor messageProcessor = this.CreateMessageProcessor();

            // act
            messageProcessor.Process();

            // assert
            Assert.IsTrue(rule1IsMatchInvoked);
            Assert.IsTrue(rule2IsMatchInvoked);
            Assert.IsTrue(rule3IsMatchInvoked);
            Assert.IsFalse(rule1ProcessInvoked);
            Assert.IsTrue(rule2ProcessInvoked);
            Assert.IsTrue(rule3ProcessInvoked);
        }

        private MessageProcessor CreateMessageProcessor()
        {
            return new MessageProcessor(
                this.ruleFactory.Object,
                this.courseManagementRepositories.Object,
                this.configurationService.Object,
                this.messageReceiver.Object);
        }
    }
}
