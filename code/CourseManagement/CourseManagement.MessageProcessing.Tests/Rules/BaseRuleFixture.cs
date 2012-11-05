using CourseManagement.MessageProcessing.Rules;
using CourseManagement.Messages;

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

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
        }

        [TestMethod]
        public void ShouldSetRuleNameWhenInitializing()
        {
            // arrange
            const string RuleName = "RuleName";
            TestableBaseRule baseRule = this.CreateBaseRule();

            RuleEntry ruleEntry = new RuleEntry(RuleName);
            baseRule.Initialize(ruleEntry);

            Assert.AreEqual(RuleName, baseRule.Name);
        }

        [TestMethod]
        public void ShouldCreateActionsThroughActionFactoryWhenRetrievingRules()
        {
            // arrange
            const string RuleName = "RuleName";
            TestableBaseRule baseRule = this.CreateBaseRule();
            RuleEntry ruleEntry = new RuleEntry(RuleName);
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
            TestableBaseRule baseRule = this.CreateBaseRule();
            RuleEntry ruleEntry = new RuleEntry(RuleName);
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
            return new TestableBaseRule(this.actionFactory.Object);
        }
    }
}
