namespace CourseManagement.MessageProcessing.Rules
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;
    using Persistence.Repositories;

    internal class AddNewTicketToDatabaseRule : BaseRule
    {
        private const string SubjectPatternTemplate = @"\[CONSULTA-{0}\][a-zA-Z\ ]*";
        
        private readonly ICourseManagementRepositories courseManagementRepositories;

        private Regex subjectRegex;

        public AddNewTicketToDatabaseRule(
            IActionFactory actionFactory, 
            ICourseManagementRepositories courseManagementRepositories) : base(actionFactory)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override void Initialize(RuleEntry ruleEntry)
        {
            base.Initialize(ruleEntry);
            bool isPublic = bool.Parse(ruleEntry.AdditionalData["public"]);
            string pattern = string.Format(SubjectPatternTemplate, isPublic ? "PUBLICA" : "PRIVADA");
            this.subjectRegex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            return this.subjectRegex.IsMatch(message.Subject);
        }
    }
}
