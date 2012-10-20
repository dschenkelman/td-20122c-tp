namespace CourseManagement.EmailProcessing
{
    using Rules;

    public class EmailProcessor
    {
        private readonly IRuleFactory ruleFactory;

        public EmailProcessor(IRuleFactory ruleFactory)
        {
            this.ruleFactory = ruleFactory;
        }

        public void Process()
        {
            this.ruleFactory.CreateRules();
        }
    }
}
