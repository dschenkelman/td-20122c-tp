namespace CourseManagement.EmailProcessing.Tests
{
    using System.Collections.Generic;
    using EmailProcessing.Rules;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class EmailProcessorFixture
    {
        private MockRepository mockRepository;
        private Mock<IRuleFactory> ruleFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.ruleFactory = this.mockRepository.Create<IRuleFactory>();
        }

        [TestMethod]
        public void ShouldRetrieveRulesFromRuleFactoryWhenProcessingEmails()
        {
            // arrange
            EmailProcessor emailProcessor = this.CreateEmailProcessor();

            this.ruleFactory.Setup(rf => rf.CreateRules()).Returns(new List<BaseRule>()).Verifiable();

            // act
            emailProcessor.Process();

            // assert
            this.ruleFactory.Verify(rf => rf.CreateRules(), Times.Once());
        }

        private EmailProcessor CreateEmailProcessor()
        {
            return new EmailProcessor(this.ruleFactory.Object);
        }
    }
}
