using System.Collections.Generic;
using System.Linq;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using CourseManagement.Utilities.Extensions;

namespace CourseManagement.MessageProcessing.Rules
{
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;

    internal class NewStudentInCourseRule : BaseRule
    {
        readonly private ICourseManagementRepositories courseManagementRepositories;

        public NewStudentInCourseRule(ICourseManagementRepositories courseManagementRepositories, IActionFactory actionFactory) : base(actionFactory)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            if( !this.subjectRegex.IsMatch(message.Subject) )
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
