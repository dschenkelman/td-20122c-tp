namespace CourseManagement.MessageProcessing.Tests.Testables
{
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Persistence.Logging;

    public class TestableBaseRule : BaseRule
    {
        public TestableBaseRule(IActionFactory actionFactory, ILogger logger)
            : base(actionFactory, logger)
        {
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            return true;
        }
    }
}
