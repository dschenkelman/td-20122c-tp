using CourseManagement.EmailProcessing.Actions;
using CourseManagement.EmailProcessing.Rules.Moles;

namespace CourseManagement.EmailProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using System.Linq;
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
        private Mock<IActionFactory> actionFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.ruleFinder = this.mockRepository.Create<IRuleFinder>();
            this.unityContainer = this.mockRepository.Create<IUnityContainer>();
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
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
        [HostType("Moles")]
        public void ShouldResolveRulesRetrievedFromFinderUsingUnityContainerWhenCreatingRulesAndReturnThem()
        {
            // arrange
            UnityRuleFactory unityRuleFactory = this.CreateUnityRuleFactory();

            const string Rule1 = "Rule1";
            const string Rule2 = "Rule2";
            const string Rule3 = "Rule3";

            var rulesToCreate = new List<string> { Rule1, Rule2, Rule3 };

            Mock<BaseRule> ruleForRule1 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            Mock<BaseRule> ruleForRule2 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            Mock<BaseRule> ruleForRule3 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);

            MBaseRule moleBaseRule1 = new MBaseRule(ruleForRule1.Object)
                                          {
                                              RetrieveActions = () => { }
                                          };

            MBaseRule moleBaseRule2 = new MBaseRule(ruleForRule2.Object)
                                          {
                                              RetrieveActions = () => { }
                                          };

            MBaseRule moleBaseRule3 = new MBaseRule(ruleForRule3.Object)
                                          {
                                              RetrieveActions = () => { }
                                          };

            this.ruleFinder.Setup(rf => rf.FindNames()).Returns(rulesToCreate).Verifiable();

            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), Rule1)).Returns(moleBaseRule1.Instance).Verifiable();
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), Rule2)).Returns(moleBaseRule2.Instance).Verifiable();
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), Rule3)).Returns(moleBaseRule3.Instance).Verifiable();

            // act
            IEnumerable<BaseRule> rules = unityRuleFactory.CreateRules();
            
            // using to list forces the yield  of the enumerable
            var rulesList = rules.ToList();

            // assert
            this.unityContainer.Verify(c => c.Resolve(typeof(BaseRule), Rule1), Times.Once());
            this.unityContainer.Verify(c => c.Resolve(typeof(BaseRule), Rule2), Times.Once());
            this.unityContainer.Verify(c => c.Resolve(typeof(BaseRule), Rule3), Times.Once());
            
            Assert.AreEqual(3, rulesList.Count);

            Assert.AreSame(ruleForRule1.Object, rulesList[0]);
            Assert.AreEqual(Rule1, rulesList[0].Name);
            Assert.AreSame(ruleForRule2.Object, rulesList[1]);
            Assert.AreEqual(Rule2, rulesList[1].Name);
            Assert.AreSame(ruleForRule3.Object, rulesList[2]);
            Assert.AreEqual(Rule3, rulesList[2].Name);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldRetrieveActionsForRules()
        {
            // arrange
            UnityRuleFactory unityRuleFactory = this.CreateUnityRuleFactory();

            const string Rule1 = "Rule1";
            const string Rule2 = "Rule2";
            const string Rule3 = "Rule3";

            var rulesToCreate = new List<string> { Rule1, Rule2, Rule3 };

            Mock<BaseRule> ruleForRule1 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            Mock<BaseRule> ruleForRule2 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            Mock<BaseRule> ruleForRule3 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);

            bool actionsRetrievedFor1 = false;
            bool actionsRetrievedFor2 = false;
            bool actionsRetrievedFor3 = false;

            MBaseRule moleBaseRule1 = new MBaseRule(ruleForRule1.Object)
            {
                RetrieveActions = () => { actionsRetrievedFor1 = true; }
            };

            MBaseRule moleBaseRule2 = new MBaseRule(ruleForRule2.Object)
            {
                RetrieveActions = () => { actionsRetrievedFor2 = true; }
            };

            MBaseRule moleBaseRule3 = new MBaseRule(ruleForRule3.Object)
            {
                RetrieveActions = () => { actionsRetrievedFor3 = true; }
            };

            this.ruleFinder.Setup(rf => rf.FindNames()).Returns(rulesToCreate);

            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), Rule1)).Returns(moleBaseRule1.Instance);
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), Rule2)).Returns(moleBaseRule2.Instance);
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), Rule3)).Returns(moleBaseRule3.Instance);

            // act
            IEnumerable<IRule> rules = unityRuleFactory.CreateRules();

            // using to list forces the yield  of the enumerable
            rules.ToList();

            // assert
            Assert.IsTrue(actionsRetrievedFor1);
            Assert.IsTrue(actionsRetrievedFor2);
            Assert.IsTrue(actionsRetrievedFor3);
        }

        private UnityRuleFactory CreateUnityRuleFactory()
        {
            return new UnityRuleFactory(this.unityContainer.Object, this.ruleFinder.Object);
        }
    }
}
