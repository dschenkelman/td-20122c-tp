using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing.Tests
{
    using System.Collections.Generic;
    using MessageProcessing.Rules;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MessageProcessorFixture
    {
        private MockRepository mockRepository;
        private Mock<IRuleFactory> ruleFactory;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.ruleFactory = this.mockRepository.Create<IRuleFactory>();
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();
        }

        [TestMethod]
        public void ShouldRetrieveRulesFromRuleFactoryWhenProcessingMessages()
        {
            // arrange
            MessageProcessor messageProcessor = this.CreateMessageProcessor();

            this.ruleFactory.Setup(rf => rf.CreateRules()).Returns(new List<BaseRule>()).Verifiable();

            // act
            messageProcessor.Process();

            // assert
            this.ruleFactory.Verify(rf => rf.CreateRules(), Times.Once());
        }

        private MessageProcessor CreateMessageProcessor()
        {
            return new MessageProcessor(this.ruleFactory.Object, this.courseManagementRepositories.Object);
        }
    }
}
