namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Utilities.Extensions;

    public class NewStudentToCourseDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagmentRepositories;

        public NewStudentToCourseDatabaseEntryAction(ICourseManagementRepositories courseManagmentRepositories)
        {
            this.courseManagmentRepositories = courseManagmentRepositories;
        }

        public void Execute(IMessage message)
        {
            int studentId = this.ParseStudentIdFromMessage(message);
            int subjectCode = this.ParseSubjectCodeFromMessage(message);
            int year = this.GetYearFromMessage(message);
            int semester = this.GetSemesterFromMessage(message);
           
            // verify
            List<Course> courses = this.courseManagmentRepositories.Courses.Get(c =>
                                                                ((c.Subject.Code == subjectCode) &&
                                                                (c.Year == year) && (c.Semester == semester))).ToList();
            if (courses.Count == 0)
            {
                throw new Exception("Subject: " + subjectCode + " is not being dictate in this semester: " + semester + "º " + year);
            }

            Course course = courses[0];

            var student = this.courseManagmentRepositories.Students.GetById(studentId);

            // perform action
            if (student == null)
            {
                student = this.CreateStudentFromMessage(message);
            }
            else
            {
                if (course.Students.Contains(student))
                {
                    throw new Exception("Student: " + student.Id + " " + student.Name + " is already in course.");
                }
            }
            
            course.Students.Add(student);
            this.courseManagmentRepositories.Courses.Save();
        }

        private Student CreateStudentFromMessage(IMessage message)
        {
            string parsedName = message.Subject.Substring(message.Subject.IndexOf("]") + 1);
            parsedName = parsedName.Substring(parsedName.IndexOf("-") + 1);

            return new Student(this.ParseStudentIdFromMessage(message), parsedName, message.From);
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
            string parsedCourse = message.Subject.Substring(0, message.Subject.IndexOf("]"));
            parsedCourse = parsedCourse.Substring(parsedCourse.LastIndexOf("-") + 1);
            return Convert.ToInt32(parsedCourse);
        }

        private int ParseStudentIdFromMessage(IMessage message)
        {
            string parsedString = message.Subject.Substring(message.Subject.IndexOf("]") + 1).Trim();
            parsedString = parsedString.Substring(0, parsedString.IndexOf("-"));
            return Convert.ToInt32(parsedString);
        }
    }
}
