namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System.Linq;
    using MessageProcessing.Actions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XmlActionReaderFixture
    {

        [TestMethod]
        [DeploymentItem(@"Files\Rules.xml", "Files")]
        public void ShouldRetrieveActionsOfARuleFormXmlFile()
        {
            // arrange
            const string PathToConfig = @"Files\Rules.xml";

            XmlActionReader actionReader = this.CreateXmlActionReader();

            const string RuleName1 = "Rule1";
            const string RuleName2 = "Rule2";
            const string RuleName3 = "Rule3";
            const string RuleName4 = "Rule4";
            
            // act
            var actionsNamesForRule1 = actionReader.GetActionEntries(PathToConfig, RuleName1).ToList();
            var actionsNamesForRule2 = actionReader.GetActionEntries(PathToConfig, RuleName2).ToList();
            var actionsNamesForRule3 = actionReader.GetActionEntries(PathToConfig, RuleName3).ToList();
            var actionsNamesForRule4 = actionReader.GetActionEntries(PathToConfig, RuleName4).ToList();

            // assert
            Assert.AreEqual(1, actionsNamesForRule1.Count);
            Assert.AreEqual("Action1", actionsNamesForRule1[0].Name);
            
            Assert.AreEqual(2, actionsNamesForRule2.Count);
            Assert.AreEqual("Action2", actionsNamesForRule2[0].Name);
            Assert.AreEqual("Action3", actionsNamesForRule2[1].Name);
            
            Assert.AreEqual(3, actionsNamesForRule3.Count);
            Assert.AreEqual("Action4", actionsNamesForRule3[0].Name);
            Assert.AreEqual("Action5", actionsNamesForRule3[1].Name);
            Assert.AreEqual("Action6", actionsNamesForRule3[2].Name);

            Assert.AreEqual(4, actionsNamesForRule4.Count);
            Assert.AreEqual("Action7", actionsNamesForRule4[0].Name);
            Assert.AreEqual("Action8", actionsNamesForRule4[1].Name);
            Assert.AreEqual("Action9", actionsNamesForRule4[2].Name);
            Assert.AreEqual("Action10", actionsNamesForRule4[3].Name);
        }

        private XmlActionReader CreateXmlActionReader()
        {
            return new XmlActionReader();
        }
    }
}
