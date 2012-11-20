namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Utilities.Extensions;

    internal class NewStudentToCourseDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagmentRepositories;

        private const string SubjectPattern = @"^\[ALTA-MATERIA-(?<subjectId>[0-9]+)\][\ ]*(?<studentId>[0-9]+)-(?<studentName>[a-zA-Z\ ]+[a-zA-Z]+)$";

        private readonly Regex subjectRegex;

        public NewStudentToCourseDatabaseEntryAction(ICourseManagementRepositories courseManagmentRepositories)
        {
            this.subjectRegex = new Regex(SubjectPattern, RegexOptions.Compiled);
            this.courseManagmentRepositories = courseManagmentRepositories;
        }

        public void Initialize(ActionEntry actionEntry)
        {
        }

        public void Execute(IMessage message)
        {
            int studentId = this.ParseStudentIdFromMessage(message);
            int subjectCode = this.ParseSubjectCodeFromMessage(message);
            int year = this.GetYearFromMessage(message);
            int semester = this.GetSemesterFromMessage(message);

            // perform action
            List<Course> courses = this.courseManagmentRepositories.Courses.Get(c =>
                                                                ((c.Subject.Code == subjectCode) &&
                                                                (c.Year == year) && (c.Semester == semester))).ToList();
            Course course = courses[0];

            var student = this.courseManagmentRepositories.Students.GetById(studentId);
            
            if (student == null)
            {
                student = this.CreateStudentFromMessage(message);
            }
            else
            {
                if (course.Students.Select(s => s.Id == student.Id ).Count() != 0)
                {
                    throw new InvalidOperationException("Student: " + student.Id + " " + student.Name + " is already in course.");
                }
            }
            
            course.Students.Add(student);
            this.courseManagmentRepositories.Courses.Save();
        }

        private Student CreateStudentFromMessage(IMessage message)
        {
            var studentName = this.subjectRegex.Match(message.Subject).Groups["studentName"].Value;
            var studentId = this.ParseStudentIdFromMessage(message);

            return new Student(studentId, studentName, message.From);
        }

        private int GetSemesterFromMessage(IMessage message)
        {
            return message.Date.Semester();
        }

        private int GetYearFromMessage(IMessage message)
        {
            return message.Date.Year;
        }

        private int ParseSubjectCodeFromMessage(IMessage message)
        {
            return Convert.ToInt32(this.subjectRegex.Match(message.Subject).Groups["subjectId"].Value);
        }

        private int ParseStudentIdFromMessage(IMessage message)
        {
            return Convert.ToInt32(this.subjectRegex.Match(message.Subject).Groups["studentId"].Value);
        }
    }
}
