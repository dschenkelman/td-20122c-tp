using System;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using MessageProcessing.Rules.Moles;
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
        private Mock<ILogger> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.ruleFinder = this.mockRepository.Create<IRuleFinder>();
            this.unityContainer = this.mockRepository.Create<IUnityContainer>();
            this.actionFactory = this.mockRepository.Create<IActionFactory>();

            this.logger = this.mockRepository.Create<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<String>()));
        }

        [TestMethod]
        public void ShouldInvokeRuleFinderToRetrieveRulesWhenCreatingRules()
        {
            // arrange
            UnityRuleFactory unityRuleFactory = this.CreateUnityRuleFactory();

            this.ruleFinder.Setup(rf => rf.FindRules()).Returns(new List<RuleEntry>()).Verifiable();

            // act
            unityRuleFactory.CreateRules(this.logger.Object);

            // assert
            this.ruleFinder.Verify(rf => rf.FindRules(), Times.Once());
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldResolveRulesRetrievedFromFinderUsingUnityContainerWhenCreatingRulesAndReturnThem()
        {
            // arrange
            UnityRuleFactory unityRuleFactory = this.CreateUnityRuleFactory();

            RuleEntry rule1 = new RuleEntry("Rule1", "RegexRule1");
            RuleEntry rule2 = new RuleEntry("Rule2", "RegexRule2");
            RuleEntry rule3 = new RuleEntry("Rule3", "RegexRule3");

            var rulesToCreate = new List<RuleEntry> { rule1, rule2, rule3 };

            Mock<BaseRule> ruleForRule1 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            ruleForRule1.Setup(r => r.Initialize(rule1)).Verifiable();
            Mock<BaseRule> ruleForRule2 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            ruleForRule2.Setup(r => r.Initialize(rule2)).Verifiable();
            Mock<BaseRule> ruleForRule3 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            ruleForRule3.Setup(r => r.Initialize(rule3)).Verifiable();

            MBaseRule moleBaseRule1 = new MBaseRule(ruleForRule1.Object)
                                          {
                                              RetrieveActions = () => { },
                                          };

            MBaseRule moleBaseRule2 = new MBaseRule(ruleForRule2.Object)
                                          {
                                              RetrieveActions = () => { }
                                          };

            MBaseRule moleBaseRule3 = new MBaseRule(ruleForRule3.Object)
                                          {
                                              RetrieveActions = () => { }
                                          };

            this.ruleFinder.Setup(rf => rf.FindRules()).Returns(rulesToCreate).Verifiable();

            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), rule1.Name)).Returns(moleBaseRule1.Instance).Verifiable();
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), rule2.Name)).Returns(moleBaseRule2.Instance).Verifiable();
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), rule3.Name)).Returns(moleBaseRule3.Instance).Verifiable();

            // act
            IEnumerable<BaseRule> rules = unityRuleFactory.CreateRules(this.logger.Object);
            
            // using to list forces the yield  of the enumerable
            var rulesList = rules.ToList();

            // assert
            this.unityContainer.Verify(c => c.Resolve(typeof(BaseRule), rule1.Name), Times.Once());
            this.unityContainer.Verify(c => c.Resolve(typeof(BaseRule), rule2.Name), Times.Once());
            this.unityContainer.Verify(c => c.Resolve(typeof(BaseRule), rule3.Name), Times.Once());
            
            Assert.AreEqual(3, rulesList.Count);

            Assert.AreSame(ruleForRule1.Object, rulesList[0]);
            Assert.AreSame(ruleForRule2.Object, rulesList[1]);
            Assert.AreSame(ruleForRule3.Object, rulesList[2]);

            ruleForRule1.Verify(r => r.Initialize(rule1), Times.Once());
            ruleForRule2.Verify(r => r.Initialize(rule2), Times.Once());
            ruleForRule3.Verify(r => r.Initialize(rule3), Times.Once());
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldRetrieveActionsForRules()
        {
            // arrange
            UnityRuleFactory unityRuleFactory = this.CreateUnityRuleFactory();

            RuleEntry rule1 = new RuleEntry("Rule1", "RegexRule1");
            RuleEntry rule2 = new RuleEntry("Rule2", "RegexRule2");
            RuleEntry rule3 = new RuleEntry("Rule3", "RegexRule3");

            var rulesToCreate = new List<RuleEntry> { rule1, rule2, rule3 };

            Mock<BaseRule> ruleForRule1 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            ruleForRule1.Setup(r => r.Initialize(rule1)).Verifiable();
            Mock<BaseRule> ruleForRule2 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            ruleForRule2.Setup(r => r.Initialize(rule2)).Verifiable();
            Mock<BaseRule> ruleForRule3 = this.mockRepository.Create<BaseRule>(this.actionFactory.Object);
            ruleForRule3.Setup(r => r.Initialize(rule3)).Verifiable();

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

            this.ruleFinder.Setup(rf => rf.FindRules()).Returns(rulesToCreate);

            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), rule1.Name)).Returns(moleBaseRule1.Instance);
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), rule2.Name)).Returns(moleBaseRule2.Instance);
            this.unityContainer.Setup(c => c.Resolve(typeof(BaseRule), rule3.Name)).Returns(moleBaseRule3.Instance);

            // act
            IEnumerable<BaseRule> rules = unityRuleFactory.CreateRules(this.logger.Object);

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
