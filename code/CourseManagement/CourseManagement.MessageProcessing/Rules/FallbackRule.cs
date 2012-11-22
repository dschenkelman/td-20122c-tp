namespace CourseManagement.MessageProcessing.Rules
{
    using Actions;
    using Messages;
    using Persistence.Logging;

    public class FallbackRule : BaseRule
    {
        public FallbackRule(IActionFactory actionFactory, ILogger logger) : base(actionFactory, logger)
        {
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            return !previouslyMatched;
        }
    }
}
