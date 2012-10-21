using System.Linq;
using CourseManagement.EmailProcessing.Actions;

namespace CourseManagement.EmailProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using Actions;
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
        public void ShouldCreateActionsThroughActionFactoryWhenRetrievingRules()
        {
            // arrange
            const string RuleName = "RuleName";
            TestableBaseRule baseRule = this.CreateBaseRule();
            baseRule.Name = RuleName;
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
        public void ShouldExecuteAllRetrievedActionsWhenProcessingEmail()
        {
            // arrange
            const string RuleName = "RuleName";
            TestableBaseRule baseRule = this.CreateBaseRule();
            baseRule.Name = RuleName;

            Mock<IAction> action1 = this.mockRepository.Create<IAction>();
            Mock<IAction> action2 = this.mockRepository.Create<IAction>();
            Mock<IAction> action3 = this.mockRepository.Create<IAction>();

            Mock<IEmail> email = this.mockRepository.Create<IEmail>();

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
