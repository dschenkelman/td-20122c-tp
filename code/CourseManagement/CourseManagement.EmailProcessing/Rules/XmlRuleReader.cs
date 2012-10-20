namespace CourseManagement.EmailProcessing.Rules
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class XmlRuleReader : IXmlRuleReader
    {
        public IEnumerable<string> GetRuleNames(string rulesConfigurationFilePath)
        {
            IEnumerable<string> names;
            using (Stream xmlStream = File.OpenRead(rulesConfigurationFilePath))
            {
                var document = XDocument.Load(xmlStream);

                // TODO: Add error handling logic
                names = document.Descendants("rule").Select(e => e.Attribute("name").Value);
            }

            return names;
        }
    }
}
