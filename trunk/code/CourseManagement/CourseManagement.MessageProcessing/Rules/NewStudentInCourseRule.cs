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
        private string subjectCodeRegex;

        public NewStudentInCourseRule(ICourseManagementRepositories courseManagementRepositories, IActionFactory actionFactory) : base(actionFactory)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.subjectCodeRegex = @"^\[ALTA-MATERIA-(?<subjectCode>[0-9]+)\][\ ]*([0-9]+)-([a-zA-Z\ ]+[a-zA-Z]+)$";
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            if( !Regex.IsMatch(message.Subject, @"^\[ALTA-MATERIA-([0-9]+)\][\ ]*([0-9]+)-([a-zA-Z\ ]+[a-zA-Z]+)$") )
            {
                return false;
            }

            int subjectCode = int.Parse(Regex.Match(message.Subject, this.subjectCodeRegex).Groups["subjectCode"].Value);
            return
                this.courseManagementRepositories.Courses.Get(
                    c =>
                    ((c.Subject.Code == subjectCode) && (c.Year == message.Date.Year) &&
                     (c.Semester == message.Date.Semester()))).ToList().Count() != 0;
        }
    }
}
