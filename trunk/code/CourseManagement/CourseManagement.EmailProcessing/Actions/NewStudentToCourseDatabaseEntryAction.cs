using System;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.EmailProcessing.Actions
{
    class NewStudentToCourseDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagmentRepositories;

        public NewStudentToCourseDatabaseEntryAction(ICourseManagementRepositories courseManagmentRepositories)
        {
            this.courseManagmentRepositories = courseManagmentRepositories;
        }

        public void Execute(IEmail email)
        {
            int studentId = this.ParseStudentIdFromEmail(email);
            int subjectCode = this.ParseSubjectCodeFromEmail(email);
            int year = this.ParseYearFromEmail(email);
            int semester = this.ParseSemesterFromEmail(email);
           
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
                student = this.CreateStudentFromEmail(email);
                this.courseManagmentRepositories.Students.Insert(student);
                this.courseManagmentRepositories.Students.Save();
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

        private Student CreateStudentFromEmail(IEmail email)
        {
            string parsedName = email.EmailSubject.Substring(email.EmailSubject.IndexOf("] ") + 2);
            parsedName = parsedName.Substring(parsedName.IndexOf("-")+1);

            return new Student(ParseStudentIdFromEmail(email), parsedName, email.Address);
        }

        private int ParseSemesterFromEmail(IEmail email)
        {
            // TODO implement
            return 2;
        }

        private int ParseYearFromEmail(IEmail email)
        {
            // TODO implement
            return 2012;
        }

        private int ParseSubjectCodeFromEmail(IEmail email)
        {
            // TODO implement
            return 7510;
        }

        private int ParseStudentIdFromEmail(IEmail email)
        {
            string parsedString = email.EmailSubject.Substring(email.EmailSubject.IndexOf("] ") + 2);
            parsedString = parsedString.Substring(0, parsedString.IndexOf("-"));
            return Convert.ToInt32(parsedString);
        }
    }
}
