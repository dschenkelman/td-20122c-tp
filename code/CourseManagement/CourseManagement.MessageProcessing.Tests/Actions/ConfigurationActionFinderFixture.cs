namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System.Collections.Generic;
    using MessageProcessing.Actions;
    using MessageProcessing.Services;
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
                .Setup(r => r.GetActionEntries(RulesConfigurationFilePathValue, NotUsedRule))
                .Returns(new List<ActionEntry>());

            ConfigurationActionFinder configurationActionFinder = this.CreateActionFinder();

            // act
            configurationActionFinder.FindActions(NotUsedRule);

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

            ActionEntry action1 = new ActionEntry("Action1");
            ActionEntry action2 = new ActionEntry("Action2");
            ActionEntry action3 = new ActionEntry("Action3");

            var retrievedActionNames = new List<ActionEntry> { action1, action2, action3 };

            this.configurationService.Setup(cs => cs.GetValue(RulesConfigurationFilePathKey))
                .Returns(RulesConfigurationFilePathValue);
            
            this.actionXmlReader
                .Setup(rcs => rcs.GetActionEntries(RulesConfigurationFilePathValue, RuleName))
                .Returns(retrievedActionNames)
                .Verifiable();

            ConfigurationActionFinder configurationActionFinder = this.CreateActionFinder();

            // act
            IEnumerable<ActionEntry> retrievedRules = configurationActionFinder.FindActions(RuleName);

            // assert
            this.actionXmlReader.Verify(rcs => rcs.GetActionEntries(RulesConfigurationFilePathValue, RuleName), Times.Once());

            Assert.AreSame(retrievedActionNames, retrievedRules);
        }

        private ConfigurationActionFinder CreateActionFinder()
        {
            return new ConfigurationActionFinder(this.configurationService.Object, this.actionXmlReader.Object);
        }
    }
}
