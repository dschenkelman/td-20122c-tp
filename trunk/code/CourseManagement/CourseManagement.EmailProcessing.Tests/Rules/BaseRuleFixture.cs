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

        private TestableBaseRule CreateBaseRule()
        {
            return new TestableBaseRule(this.actionFactory.Object);
        }
    }
}
