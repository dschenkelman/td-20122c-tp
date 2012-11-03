using System;
using System.Text.RegularExpressions;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;

namespace CourseManagement.MessageProcessing.Rules
{
    class NewStudentInCourseRule :BaseRule
    {
        public NewStudentInCourseRule(IActionFactory actionFactory) : base(actionFactory)
        {
            this.Name = "";
        }

        public override bool IsMatch(IMessage message)
        {

            return Regex.IsMatch(message.Subject, @"^\[ALTA-MATERIA-[0-9]+\][0-9]+-[a-zA-Z\ ]+[a-zA-Z]+$");
        }
    }
}
