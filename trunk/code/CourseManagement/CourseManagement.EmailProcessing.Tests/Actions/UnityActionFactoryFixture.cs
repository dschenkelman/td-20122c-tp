namespace CourseManagement.EmailProcessing.Tests.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using EmailProcessing.Actions;
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

            this.actionFinder.Setup(af => af.FindNames(RuleName)).Returns(new List<string>()).Verifiable();

            // act
            unityActionFactory.CreateActions(RuleName);

            // assert
            this.actionFinder.Verify(af => af.FindNames(RuleName), Times.Once());
        }

        [TestMethod]
        public void ShouldRetrieveActionFromContainerMappedFromStringReturnedByFindNamesWhenUsingCreateActions()
        {
            // arrange
            UnityActionFactory unityActionFactory = this.CreateActionFactory();
            
            const string RuleName = "Rule1";
            const string Action1 = "Action1";
            const string Action2 = "Action2";

            Mock<IAction> action1ForRule1 = this.mockRepository.Create<IAction>();
            Mock<IAction> action2ForRule1 = this.mockRepository.Create<IAction>();

            List<string> actions = new List<string> { Action1, Action2 };
            this.actionFinder.Setup(af => af.FindNames(RuleName)).Returns(actions);

            this.container.Setup(ct => ct.Resolve(typeof(IAction), Action1)).Returns(action1ForRule1.Object);
            this.container.Setup(ct => ct.Resolve(typeof(IAction), Action2)).Returns(action2ForRule1.Object);

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
