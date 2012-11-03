using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;

namespace CourseManagement.MessageProcessing.Rules
{
    class AddDeliverableToGroupDatabaseEntryRule : BaseRule
    {
        public AddDeliverableToGroupDatabaseEntryRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }

        public override bool IsMatch(IMessage message)
        {
            return Regex.IsMatch(message.Subject, @"^\[ENTREGA-TP-[0-9]+\]$") && message.Attachments.Count() > 0 ;
        }
    }
}
