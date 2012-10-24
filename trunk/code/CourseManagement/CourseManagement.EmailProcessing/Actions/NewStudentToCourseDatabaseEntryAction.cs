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
            Student student = ParseStudentFromEmail(email);
            int subjectCode = ParseSubjectCodeFromEmail(email);
            int year = ParseYearFromEmail(email);
            int semester = ParseSemesterFromEmail(email);
           
            // verify
            List<Course> courses = this.courseManagmentRepositories.Courses.Get(c =>
                                                                ((c.Subject.Code == subjectCode) &&
                                                                (c.Year == year) && (c.Semester == semester))).ToList();
            if (courses.Count() == 0)
            {
                throw new Exception("Subject: " + subjectCode + " is not being dictate in this semester: " + semester + "º " + year);
            }
            if (this.courseManagmentRepositories.Students.GetById(student.Id) != null)
            {
                if (courses[0].Students.Contains(this.courseManagmentRepositories.Students.GetById(student.Id)))
                {
                    throw new Exception("Student: " + student.Id + " " + student.Name + " is already in course.");
                }
            }

            // perform action
            if (this.courseManagmentRepositories.Students.GetById(student.Id) == null)
            {
                this.courseManagmentRepositories.Students.Insert(student);
                this.courseManagmentRepositories.Students.Save();
            }
            courses[0].Students.Add(student);
            this.courseManagmentRepositories.Courses.Save();
        }

        private int ParseSemesterFromEmail(IEmail email)
        {
            return 2;
        }

        private int ParseYearFromEmail(IEmail email)
        {
            return 2012;
        }

        private int ParseSubjectCodeFromEmail(IEmail email)
        {
            return 7510;
        }

        private Student ParseStudentFromEmail(IEmail email)
        {
            return new Student(91363, "Matias Servetto", "servetto.matias@gmail.com");
        }
    }
}
