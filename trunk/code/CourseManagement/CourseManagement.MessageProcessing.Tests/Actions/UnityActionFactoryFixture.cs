namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using MessageProcessing.Actions;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class UnityActionFactoryFixture
    {
        private MockRepository mockRepository;
        private Mock<IActionFinder> actionFinder;
        private Mock<IUnityContainer> container;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFinder = this.mockRepository.Create<IActionFinder>();
            this.container = this.mockRepository.Create<IUnityContainer>();
        }

        [TestMethod]
        public void ShouldUseFindNamesFromActionFinderWhenUsingCreateActions()
        {
            // arrange
            UnityActionFactory unityActionFactory = this.CreateActionFactory();

            const string RuleName = "Rule1";

            this.actionFinder.Setup(af => af.FindActions(RuleName)).Returns(new List<ActionEntry>()).Verifiable();

            // act
            unityActionFactory.CreateActions(RuleName);

            // assert
            this.actionFinder.Verify(af => af.FindActions(RuleName), Times.Once());
        }

        [TestMethod]
        public void ShouldRetrieveActionFromContainerMappedFromStringReturnedByFindNamesWhenUsingCreateActions()
        {
            // arrange
            UnityActionFactory unityActionFactory = this.CreateActionFactory();
            
            const string RuleName = "Rule1";
            ActionEntry action1 = new ActionEntry("Action1");
            ActionEntry action2 = new ActionEntry("Action2");

            Mock<IAction> action1ForRule1 = this.mockRepository.Create<IAction>();
            Mock<IAction> action2ForRule1 = this.mockRepository.Create<IAction>();

            List<ActionEntry> actions = new List<ActionEntry> { action1, action2 };
            this.actionFinder.Setup(af => af.FindActions(RuleName)).Returns(actions);

            this.container.Setup(ct => ct.Resolve(typeof(IAction), action1.Name)).Returns(action1ForRule1.Object);
            this.container.Setup(ct => ct.Resolve(typeof(IAction), action2.Name)).Returns(action2ForRule1.Object);

            // act
            IEnumerable<IAction> actionsRetrieved = unityActionFactory.CreateActions(RuleName);
            var actionsRetrievedList = actionsRetrieved.ToList();

            // assert
            Assert.AreSame(action1ForRule1.Object, actionsRetrievedList[0]);
        }

        private UnityActionFactory CreateActionFactory()
        {
            return new UnityActionFactory(this.container.Object, this.actionFinder.Object);
        }
    }
}
