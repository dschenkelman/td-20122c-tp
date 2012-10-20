namespace CourseManagement.EmailProcessing.Tests.Actions
{
    using System.Collections.Generic;
    using EmailProcessing.Actions;
    using EmailProcessing.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ConfigurationActionFinderFixture
    {
        private MockRepository mockRepository;
        private Mock<IXmlActionReader> actionXmlReader;
        private Mock<IConfigurationService> configurationService;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionXmlReader = this.mockRepository.Create<IXmlActionReader>();
            this.configurationService = this.mockRepository.Create<IConfigurationService>();
        }

        [TestMethod]
        public void ShouldGetFileToRetrieveRulesFromThroughConfigurationService()
        {
            // arrange
            const string RulesConfigurationFilePathKey = "RulesConfigurationFilePath";

            const string RulesConfigurationFilePathValue = "Rules.xml";

            const string NotUsedRule = "Not Used";

            this.configurationService.Setup(cs => cs.GetValue(RulesConfigurationFilePathKey))
                .Returns(RulesConfigurationFilePathValue).Verifiable();

            this.actionXmlReader
                .Setup(r => r.GetActionNames(RulesConfigurationFilePathValue, NotUsedRule))
                .Returns(new List<string>());

            ConfigurationActionFinder configurationActionFinder = this.CreateActionFinder();

            // act
            configurationActionFinder.FindNames(NotUsedRule);

            // assert
            this.configurationService.Verify(cs => cs.GetValue(RulesConfigurationFilePathKey), Times.Once());
        }

        [TestMethod]
        public void ShouldGetRulesNamesFromRulesConfigurationServiceUsingFileNameRetrievedFromConfigurationService()
        {
            // arrange
            const string RulesConfigurationFilePathKey = "RulesConfigurationFilePath";

            const string RulesConfigurationFilePathValue = "Rules.xml";

            const string RuleName = "Rule";

            const string Action1 = "Action1";
            const string Action2 = "Action2";
            const string Action3 = "Action3";

            var retrievedActionNames = new List<string> { Action1, Action2, Action3 };

            this.configurationService.Setup(cs => cs.GetValue(RulesConfigurationFilePathKey))
                .Returns(RulesConfigurationFilePathValue);
            
            this.actionXmlReader
                .Setup(rcs => rcs.GetActionNames(RulesConfigurationFilePathValue, RuleName))
                .Returns(retrievedActionNames)
                .Verifiable();

            ConfigurationActionFinder configurationActionFinder = this.CreateActionFinder();

            // act
            IEnumerable<string> retrievedRules = configurationActionFinder.FindNames(RuleName);

            // assert
            this.actionXmlReader.Verify(rcs => rcs.GetActionNames(RulesConfigurationFilePathValue, RuleName), Times.Once());

            Assert.AreSame(retrievedActionNames, retrievedRules);
        }

        private ConfigurationActionFinder CreateActionFinder()
        {
            return new ConfigurationActionFinder(this.configurationService.Object, this.actionXmlReader.Object);
        }
    }
}
