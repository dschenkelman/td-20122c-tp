namespace CourseManagement.MessageProcessing.Rules
{
    using System.Collections.Specialized;

    public class RuleEntry
    {
        public RuleEntry(string name)
        {
            this.AdditionalData = new NameValueCollection();
            this.Name = name;
        }

        public NameValueCollection AdditionalData { get; private set; }

        public string Name { get; private set; }
    }
}