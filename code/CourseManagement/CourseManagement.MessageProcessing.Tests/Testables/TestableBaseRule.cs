using System;
using CourseManagement.Messages;

namespace CourseManagement.MessageProcessing.Tests.Testables
{
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;

    public class TestableBaseRule : BaseRule
    {
        public TestableBaseRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }

        public override bool IsMatch(IMessage message)
        {
            return true;
        }
    }
}
