namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System.Linq;
    using MessageProcessing.Rules;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XmlRuleReaderFixture
    {
        [TestMethod]
        [DeploymentItem(@"Files\Rules.xml", "Files")]
        public void ShouldRetrieveRulesFromXmlFile()
        {
            // arrange
            const string PathToConfig = @"Files\Rules.xml";

            XmlRuleReader ruleReader = this.CreateXmlRuleReader();

            // act
            var ruleNames = ruleReader.GetRuleNames(PathToConfig).ToList();

            // assert
            Assert.AreEqual(4, ruleNames.Count);
            Assert.AreEqual("Rule1", ruleNames[0].Name);
            Assert.AreEqual("Rule2", ruleNames[1].Name);
            Assert.AreEqual("Rule3", ruleNames[2].Name);
            Assert.AreEqual("Rule4", ruleNames[3].Name);
        }

        private XmlRuleReader CreateXmlRuleReader()
        {
            return new XmlRuleReader();
        }
    }
}
