namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class XmlActionReader : IXmlActionReader
    {
        public IEnumerable<string> GetActionNames(string rulesConfigurationFilePath, string ruleName)
        {
            IEnumerable<string> names;
            using (Stream xmlStream = File.OpenRead(rulesConfigurationFilePath))
            {
                var document = XDocument.Load(xmlStream);

                // TODO: Add error handling logic
                names = document.Descendants("rule")
                    .Where(e => e.Attribute("name").Value.Equals(ruleName))
                    .Descendants("action")
                    .Select(e => e.Attribute("name").Value);
            }

            return names;
        }
    }
}
