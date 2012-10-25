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

            var group = new Group();

            //verify existing group
            var groupsInRepository = this.courseManagementRepositories.Groups.Get(g => g.Id == group.Id ).ToList();
            
            if ( groupsInRepository.Count != 0)
            {
                throw new Exception("Trying to add to database an already existing group ");
            }

            this.courseManagementRepositories.Groups.Insert(group);

            this.courseManagementRepositories.Groups.Save();

        }

    }
}
