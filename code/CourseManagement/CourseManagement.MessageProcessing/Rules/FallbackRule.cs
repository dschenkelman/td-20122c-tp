namespace CourseManagement.MessageProcessing.Rules
{
    using Actions;
    using Messages;

    public class FallbackRule : BaseRule
    {
        public FallbackRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }

        public override bool IsMatch(IMessage message)
        {
            return true;
        }
    }
}
