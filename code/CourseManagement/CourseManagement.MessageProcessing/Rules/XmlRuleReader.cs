using CourseManagement.Utilities.Extensions;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class XmlRuleReader : IXmlRuleReader
    {
        public IEnumerable<RuleEntry> GetRuleNames(string rulesConfigurationFilePath)
        {
            IEnumerable<RuleEntry> ruleEntries;
            using (Stream xmlStream = File.OpenRead(rulesConfigurationFilePath))
            {
                var document = XDocument.Load(xmlStream);

                // TODO: Add error handling logic
                ruleEntries = document.Descendants("rule").Select(e =>
                    {
                        var ruleEntry = new RuleEntry(e.Attribute("name").Value);
                        e.Attributes().Where(a => a.Name != "name").ForEach(
                            a => ruleEntry.AdditionalData.Add(a.Name.LocalName, a.Value));
                        return ruleEntry;
                    });
            }

            return ruleEntries;
        }
    }
}
