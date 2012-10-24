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
            int year = ParseYearFromEmail(email);
            int semester = ParseSemesterFromEmail(email);

            //TODO bajar los attachments

            Group group = new Group(year, semester);

            //verify existing group
            var groupsInRepository = this.courseManagementRepositories.Groups.Get(g => (g.Semester == group.Semester) &&
                                                                                       (g.Year == group.Year) &&
                                                                                      (g.Id == group.Id) ).ToList();
            if ( groupsInRepository == null)
            {
                Console.WriteLine("Es null");
                Console.ReadLine();
            }
            if ( groupsInRepository.Count != 0)
            {
                throw new Exception("Trying to add to database an already existing group ");
            }

            this.courseManagementRepositories.Groups.Insert(group);

            this.courseManagementRepositories.Groups.Save();

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
    }
}
