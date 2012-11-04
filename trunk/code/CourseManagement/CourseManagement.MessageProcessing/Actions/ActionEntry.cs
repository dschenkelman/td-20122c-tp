namespace CourseManagement.MessageProcessing.Actions
{
    using System.Collections.Specialized;

    public class ActionEntry
    {
        public ActionEntry(string name)
        {
            this.AdditionalData = new NameValueCollection();
            this.Name = name;
        }
        
        public NameValueCollection AdditionalData { get; private set; }

        public string Name { get; private set; }
    }
}
