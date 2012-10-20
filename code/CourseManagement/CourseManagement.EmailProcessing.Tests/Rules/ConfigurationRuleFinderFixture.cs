namespace CourseManagement.EmailProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using EmailProcessing.Rules;
    using EmailProcessing.Services;
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
                .Returns(new List<string>());

            ConfigurationRuleFinder configurationRuleFinder = this.CreateConfigurationRuleFinder();

            // act
            configurationRuleFinder.FindNames();

            // assert
            this.configurationService.Verify(cs => cs.GetValue(RulesConfigurationFilePathKey), Times.Once());
        }

        [TestMethod]
        public void ShouldGetRulesNamesFromRulesConfigurationServiceUsingFileNameRetrievedFromConfigurationService()
        {
            // arrange
            const string RulesConfigurationFilePathKey = "RulesConfigurationFilePath";

            const string RulesConfigurationFilePathValue = "Rules.xml";

            const string Rule1 = "Rule1";
            const string Rule2 = "Rule2";
            const string Rule3 = "Rule3";

            var retrievedRuleNames = new List<string> { Rule1, Rule2, Rule3 };

            this.configurationService.Setup(cs => cs.GetValue(RulesConfigurationFilePathKey))
                .Returns(RulesConfigurationFilePathValue);

            this.ruleXmlReader
                .Setup(rcs => rcs.GetRuleNames(RulesConfigurationFilePathValue))
                .Returns(retrievedRuleNames)
                .Verifiable();

            ConfigurationRuleFinder configurationRuleFinder = this.CreateConfigurationRuleFinder();

            // act
            IEnumerable<string> retrievedRules = configurationRuleFinder.FindNames();

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
