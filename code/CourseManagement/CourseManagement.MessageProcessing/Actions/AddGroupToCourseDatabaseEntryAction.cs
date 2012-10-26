namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Linq;
    using Model;
    using Persistence.Repositories;

    public class AddGroupToCourseDatabaseEntryAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public AddGroupToCourseDatabaseEntryAction(ICourseManagementRepositories courseManagementRepositories)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public void Execute(IMessage message)
        {
            // Add group
            // TODO bajar los attachments
            Course course = this.GetCourseFromMessage(message);

            var group = new Group { CourseId = course.Id, Course = course };

            // verify existing group
            var groupsInRepository = this.courseManagementRepositories.Groups.Get(g => (g.Id == group.Id) && (g.CourseId == group.CourseId)).ToList();
            
            if (groupsInRepository.Count != 0)
            {
                throw new Exception("Trying to add to database an already existing group ");
            }

            this.courseManagementRepositories.Groups.Insert(group);

            this.courseManagementRepositories.Groups.Save();

            // TODO Add Students to New Group
        }

        private Course GetCourseFromMessage(IMessage message)
        {
            int subjectCode = this.ParseSubjectCodeFromMessage(message);

            int year = message.Date.Year;
            int semester = this.GetSemesterFromMessage(message);

            var courses = this.courseManagementRepositories.Courses.Get(c => (c.SubjectId == subjectCode)
                                            && (c.Year == year) && (c.Semester == semester)).ToList();

            var course = courses.ElementAt(0);

            return course;
        }

        private int ParseSubjectCodeFromMessage(IMessage message)
        {
            string userAccount = message.Address;
            
            var accounts = this.courseManagementRepositories.Accounts.Get(a => a.User == userAccount).ToList();
            int subjectCode = accounts.ElementAt(0).SubjectCode;


            return subjectCode;
        }

        public int GetSemesterFromMessage(IMessage message)
        {
            return message.Date.Month <= 1 && message.Date.Month <= 6 ? 1 : 2;
        }
    }
}
