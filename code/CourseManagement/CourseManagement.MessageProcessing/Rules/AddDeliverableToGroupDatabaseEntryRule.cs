namespace CourseManagement.MessageProcessing.Rules
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;

    class AddDeliverableToGroupDatabaseEntryRule : BaseRule
    {
        public AddDeliverableToGroupDatabaseEntryRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            return Regex.IsMatch(message.Subject, @"^\[ENTREGA-TP-[0-9]+\]$") && message.Attachments.Count() > 0 ;
        }
    }
}
