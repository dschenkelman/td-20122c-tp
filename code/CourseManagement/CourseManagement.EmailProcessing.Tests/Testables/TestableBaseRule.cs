using CourseManagement.EmailProcessing.Actions;

namespace CourseManagement.EmailProcessing.Tests.Testables
{
    using Actions;
    using EmailProcessing.Rules;

    public class TestableBaseRule : BaseRule
    {
        public TestableBaseRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }
    }
}
