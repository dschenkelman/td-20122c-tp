using System.Linq;

namespace CourseManagement.EmailProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using EmailProcessing.Rules;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class UnityRuleFactoryFixture
    {
        private MockRepository mockRepository;
        private Mock<IRuleFinder> ruleFinder;
        private Mock<IUnityContainer> unityContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.ruleFinder = this.mockRepository.Create<IRuleFinder>();
            this.unityContainer = this.mockRepository.Create<IUnityContainer>();
        }

        [TestMethod]
        public void ShouldInvokeRuleFinderToRetrieveRulesWhenCreatingRules()
        {
            // arrange
            UnityRuleFactory unityRuleFactory = this.CreateUnityRuleFactory();

            this.ruleFinder.Setup(rf => rf.FindNames()).Returns(new List<string>()).Verifiable();

            // act
            unityRuleFactory.CreateRules();

            // assert
            this.ruleFinder.Verify(rf => rf.FindNames(), Times.Once());
        }

        [TestMethod]
        public void ShouldResolveRulesRetrievedFromFinderUsingUnityContainerWhenCreatingRulesAndReturnThem()
        {
            // arrange
            UnityRuleFactory unityRuleFactory = this.CreateUnityRuleFactory();

            const string Rule1 = "Rule1";
            const string Rule2 = "Rule2";
            const string Rule3 = "Rule3";

            var rulesToCreate = new List<string> { Rule1, Rule2, Rule3 };

            Mock<IRule> ruleForRule1 = this.mockRepository.Create<IRule>();
            Mock<IRule> ruleForRule2 = this.mockRepository.Create<IRule>();
            Mock<IRule> ruleForRule3 = this.mockRepository.Create<IRule>();

            this.ruleFinder.Setup(rf => rf.FindNames()).Returns(rulesToCreate).Verifiable();

            this.unityContainer.Setup(c => c.Resolve(typeof(IRule), Rule1)).Returns(ruleForRule1.Object).Verifiable();
            this.unityContainer.Setup(c => c.Resolve(typeof(IRule), Rule2)).Returns(ruleForRule2.Object).Verifiable();
            this.unityContainer.Setup(c => c.Resolve(typeof(IRule), Rule3)).Returns(ruleForRule3.Object).Verifiable();

            // act
            IEnumerable<IRule> rules = unityRuleFactory.CreateRules();
            
            // using to list forces the yield  of the enumerable
            var rulesList = rules.ToList();

            // assert
            this.unityContainer.Verify(c => c.Resolve(typeof(IRule), Rule1), Times.Once());
            this.unityContainer.Verify(c => c.Resolve(typeof(IRule), Rule2), Times.Once());
            this.unityContainer.Verify(c => c.Resolve(typeof(IRule), Rule3), Times.Once());
            
            Assert.AreEqual(3, rulesList.Count);

            Assert.AreEqual(ruleForRule1.Object, rulesList[0]);
            Assert.AreEqual(ruleForRule2.Object, rulesList[1]);
            Assert.AreEqual(ruleForRule3.Object, rulesList[2]);
        }

        private UnityRuleFactory CreateUnityRuleFactory()
        {
            return new UnityRuleFactory(this.unityContainer.Object, this.ruleFinder.Object);
        }
    }
}
