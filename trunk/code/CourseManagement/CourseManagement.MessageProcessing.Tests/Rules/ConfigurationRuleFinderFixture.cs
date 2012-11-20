namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using MessageProcessing.Rules;
    using MessageProcessing.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ConfigurationRuleFinderFixture
    {
        private MockRepository mockRepository;
        private Mock<IXmlRuleReader> ruleXmlReader;
        private Mock<IConfigurationService> configurationService;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.ruleXmlReader = this.mockRepository.Create<IXmlRuleReader>();
            this.configurationService = this.mockRepository.Create<IConfigurationService>();
        }

        [TestMethod]
        public void ShouldGetFileToRetrieveRulesFromThroughConfigurationService()
        {
            // arrange
            const string RulesConfigurationFilePathKey = "RulesConfigurationFilePath";

            const string RulesConfigurationFilePathValue = "Rules.xml";

            this.configurationService.Setup(cs => cs.GetValue(RulesConfigurationFilePathKey))
                .Returns(RulesConfigurationFilePathValue).Verifiable();

            this.ruleXmlReader
                .Setup(rcs => rcs.GetRuleNames(RulesConfigurationFilePathValue))
                .Returns(new List<RuleEntry>());

            ConfigurationRuleFinder configurationRuleFinder = this.CreateConfigurationRuleFinder();

            // act
            configurationRuleFinder.FindRules();

            // assert
            this.configurationService.Verify(cs => cs.GetValue(RulesConfigurationFilePathKey), Times.Once());
        }

        [TestMethod]
        public void ShouldGetRulesNamesFromRulesConfigurationServiceUsingFileNameRetrievedFromConfigurationService()
        {
            // arrange
            const string RulesConfigurationFilePathKey = "RulesConfigurationFilePath";

            const string RulesConfigurationFilePathValue = "Rules.xml";

            RuleEntry rule1 = new RuleEntry("Rule1", "RegexRule1");
            RuleEntry rule2 = new RuleEntry("Rule2", "RegexRule2");
            RuleEntry rule3 = new RuleEntry("Rule3", "RegexRule3");

            var retrievedRuleNames = new List<RuleEntry> { rule1, rule2, rule3 };

            this.configurationService.Setup(cs => cs.GetValue(RulesConfigurationFilePathKey))
                .Returns(RulesConfigurationFilePathValue);

            this.ruleXmlReader
                .Setup(rcs => rcs.GetRuleNames(RulesConfigurationFilePathValue))
                .Returns(retrievedRuleNames)
                .Verifiable();

            ConfigurationRuleFinder configurationRuleFinder = this.CreateConfigurationRuleFinder();

            // act
            IEnumerable<RuleEntry> retrievedRules = configurationRuleFinder.FindRules();

            // assert
            this.ruleXmlReader.Verify(rcs => rcs.GetRuleNames(RulesConfigurationFilePathValue), Times.Once());
            
            Assert.AreSame(retrievedRuleNames, retrievedRules);
        }

        private ConfigurationRuleFinder CreateConfigurationRuleFinder()
        {
            return new ConfigurationRuleFinder(this.configurationService.Object, this.ruleXmlReader.Object);
        }
    }
}
