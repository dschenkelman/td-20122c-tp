namespace CourseManagement.MessageProcessing.Rules
{
    using System.Linq;
    using Actions;
    using Messages;
    using Persistence.Logging;
    using Persistence.Repositories;
    using Utilities.Extensions;

    internal class NewStudentInCourseRule : BaseRule
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public NewStudentInCourseRule(
            ICourseManagementRepositories courseManagementRepositories,
            IActionFactory actionFactory,
            ILogger logger)
            : base(actionFactory, logger)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            if (!this.subjectRegex.IsMatch(message.Subject))
            {
                return false;
            }

            int year = message.Date.Year;
            int semester = message.Date.Semester();
            int subjectCode = int.Parse(this.subjectRegex.Match(message.Subject).Groups["subjectCode"].Value);
            return
                this.courseManagementRepositories.Courses.Get(
                    c =>
                    ((c.Subject.Code == subjectCode) && (c.Year == year) &&
                     (c.Semester == semester))).ToList().Count() != 0;
        }
    }
}
