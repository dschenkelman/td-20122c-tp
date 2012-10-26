namespace CourseManagement.MessageProcessing.Tests.Testables
{
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;

    public class TestableBaseRule : BaseRule
    {
        public TestableBaseRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }
    }
}
