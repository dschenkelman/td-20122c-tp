namespace CourseManagement.MessageProcessing.Rules
{
    using System.Text.RegularExpressions;
    using Actions;
    using Messages;

    internal class NewStudentInCourseRule :BaseRule
    {
        public NewStudentInCourseRule(IActionFactory actionFactory) : base(actionFactory)
        {
            this.Name = string.Empty;
        }

        public override bool IsMatch(IMessage message)
        {
            return Regex.IsMatch(message.Subject, @"^\[ALTA-MATERIA-([0-9]+)\][\ ]*([0-9]+)-([a-zA-Z\ ]+[a-zA-Z]+)$");
        }
    }
}
