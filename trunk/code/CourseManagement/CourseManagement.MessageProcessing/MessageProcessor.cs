namespace CourseManagement.MessageProcessing
{
    using Rules;

    public class MessageProcessor
    {
        private readonly IRuleFactory ruleFactory;

        public MessageProcessor(IRuleFactory ruleFactory)
        {
            this.ruleFactory = ruleFactory;
        }

        public void Process()
        {
            this.ruleFactory.CreateRules();
        }
    }
}
