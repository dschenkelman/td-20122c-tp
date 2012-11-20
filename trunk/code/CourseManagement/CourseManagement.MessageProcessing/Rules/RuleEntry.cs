using System;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Specialized;

    public class RuleEntry
    {
        public RuleEntry(string name, string subjectRegex)
        {
            this.AdditionalData = new NameValueCollection();
            this.Name = name;
            this.SubjectRegex = subjectRegex;
        }

        public NameValueCollection AdditionalData { get; private set; }

        public string Name { get; private set; }
        public string SubjectRegex { get; private set; }

    }
}