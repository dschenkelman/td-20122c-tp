using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.EmailProcessing.Actions
{
    class AddGroupToCourseDatabaseEntryAction
    {
        private ICourseManagementRepositories courseManagementRepositories;

        public AddGroupToCourseDatabaseEntryAction(ICourseManagementRepositories courseManagementRepositories)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public void Execute(IEmail email)
        {

            // Add group
            //TODO bajar los attachments

            Course course = ObtainCourseFromEmail(email);

            var group = new Group {CourseId = course.Id , Course = course};

            //verify existing group
            var groupsInRepository = this.courseManagementRepositories.Groups.Get(g => (g.Id == group.Id) && (g.CourseId == group.CourseId)).ToList();
            
            if ( groupsInRepository.Count != 0)
            {
                throw new Exception("Trying to add to database an already existing group ");
            }

            this.courseManagementRepositories.Groups.Insert(group);

            this.courseManagementRepositories.Groups.Save();

            //TODO Add Students to New Group
        }

        private Course ObtainCourseFromEmail(IEmail email)
        {
            int subjectCode = ParseSubjectCodeFromEmail(email);

            int year = email.Date.Year;
            int semester = ParseSemesterFromMonth(email.Date.Month);

            var courses = this.courseManagementRepositories.Courses.Get(c => (c.SubjectId == subjectCode)
                                            && (c.Year == year) && (c.Semester == semester)).ToList();

            var course = courses.ElementAt(0);

            return course;
        }

        private int ParseSubjectCodeFromEmail(IEmail email)
        {
            string userAccount = email.Address;
            
            var accounts = this.courseManagementRepositories.Accounts.Get(a => a.User == userAccount).ToList();
            int subjectCode = accounts.ElementAt(0).SubjectCode;


            return subjectCode;
        }

        public int ParseSemesterFromMonth(int month)
        {
            if ((1 <= month) && (month <= 6))
                return 1;
            return 2;
        }
    }
}
