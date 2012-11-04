namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Utilities.Extensions;

    public class XmlActionReader : IXmlActionReader
    {
        public IEnumerable<ActionEntry> GetActionEntries(string rulesConfigurationFilePath, string ruleName)
        {
            IEnumerable<ActionEntry> names;
            using (Stream xmlStream = File.OpenRead(rulesConfigurationFilePath))
            {
                var document = XDocument.Load(xmlStream);

                // TODO: Add error handling logic
                names = document.Descendants("rule")
                    .Where(e => e.Attribute("name").Value.Equals(ruleName))
                    .Descendants("action")
                    .Select(e => 
                    {
                        var actionEntry = new ActionEntry(e.Attribute("name").Value);
                        e.Attributes().Where(a => a.Name != "name").ForEach(
                            a => actionEntry.AdditionalData.Add(a.Name.LocalName, a.Value));

                        return actionEntry;
                    });
            }

            return names;
        }
    }
}
