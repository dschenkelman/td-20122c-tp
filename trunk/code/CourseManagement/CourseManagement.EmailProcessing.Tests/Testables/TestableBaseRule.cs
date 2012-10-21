namespace CourseManagement.EmailProcessing.Tests.Testables
{
    using EmailProcessing.Actions;
    using EmailProcessing.Rules;

    public class TestableBaseRule : BaseRule
    {
        public TestableBaseRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }
    }
}
