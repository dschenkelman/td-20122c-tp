namespace CourseManagement.MessageProcessing.Tests.Testables
{
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;

    public class TestableBaseRule : BaseRule
    {
        public TestableBaseRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            return true;
        }
    }
}
