using System;
using CourseManagement.MessageProcessing.Rules;
using CourseManagement.Messages;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using MessageProcessing.Actions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Testables;

    [TestClass]
    public class BaseRuleFixture
    {
        private MockRepository mockRepository;
        private Mock<IActionFactory> actionFactory;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.logger = this.mockRepository.Create<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<String>()));
        }

        [TestMethod]
        public void ShouldSetRuleNameWhenInitializing()
        {
            // arrange
            const string RuleName = "RuleName";
            const string RuleSubjectRegex = "RuleRegex";
            TestableBaseRule baseRule = this.CreateBaseRule();

            RuleEntry ruleEntry = new RuleEntry(RuleName, RuleSubjectRegex);
            baseRule.Initialize(ruleEntry);

            Assert.AreEqual(RuleName, baseRule.Name);
        }

        [TestMethod]
        public void ShouldCreateActionsThroughActionFactoryWhenRetrievingRules()
        {
            // arrange
            const string RuleName = "RuleName";
            const string RuleSubjectRegex = "RuleRegex";
            TestableBaseRule baseRule = this.CreateBaseRule();
            RuleEntry ruleEntry = new RuleEntry(RuleName, RuleSubjectRegex);
            baseRule.Initialize(ruleEntry);
            this.actionFactory
                .Setup(af => af.CreateActions(RuleName))
                .Returns(new List<IAction>())
                .Verifiable();

            // act
            baseRule.RetrieveActions();

            // assert
            this.actionFactory.Verify(af => af.CreateActions(RuleName), Times.Once());
        }

        [TestMethod]
        public void ShouldExecuteAllRetrievedActionsWhenProcessingMessage()
        {
            // arrange
            const string RuleName = "RuleName";
            const string RuleSubjectRegex = "RuleRegex";
            TestableBaseRule baseRule = this.CreateBaseRule();
            RuleEntry ruleEntry = new RuleEntry(RuleName, RuleSubjectRegex);
            baseRule.Initialize(ruleEntry);

            Mock<IAction> action1 = this.mockRepository.Create<IAction>();
            Mock<IAction> action2 = this.mockRepository.Create<IAction>();
            Mock<IAction> action3 = this.mockRepository.Create<IAction>();

            Mock<IMessage> email = this.mockRepository.Create<IMessage>();

            var actions = new List<Mock<IAction>> { action1, action2, action3 };

            this.actionFactory
                .Setup(af => af.CreateActions(RuleName))
                .Returns(actions.Select(a => a.Object))
                .Verifiable();

            actions.ForEach(a => a.Setup(act => act.Execute(email.Object)).Verifiable());

            // act
            baseRule.RetrieveActions();

            actions.ForEach(a => a.Verify(act => act.Execute(email.Object), Times.Never()));

            baseRule.Process(email.Object);

            // assert
            actions.ForEach(a => a.Verify(act => act.Execute(email.Object), Times.Once()));
        }

        private TestableBaseRule CreateBaseRule()
        {
            return new TestableBaseRule(this.actionFactory.Object, this.logger.Object);
        }
    }
}
