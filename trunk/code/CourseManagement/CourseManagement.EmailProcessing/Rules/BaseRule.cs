namespace CourseManagement.EmailProcessing.Rules
{
    using Actions;

    public abstract class BaseRule : IRule
    {
        private readonly IActionFactory actionFactory;

        protected BaseRule(IActionFactory actionFactory)
        {
            this.actionFactory = actionFactory;
        }

        public string Name { get; set; }

        public void RetrieveActions()
        {
            this.actionFactory.CreateActions(this.Name);
        }
    }
}
