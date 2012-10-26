using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing.Actions
{
    class AddDerivableToGroupDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagmentRepositories;

        public AddDerivableToGroupDatabaseEntryAction(ICourseManagementRepositories courseManagmentRepositories)
        {
            this.courseManagmentRepositories = courseManagmentRepositories;
        }

        public void Execute(IMessage message)
        {
            var students = this.courseManagmentRepositories.Students.Get( s => s.MessagingSystemId == message.Address );
            var studentGroup = students.ElementAt(0).Groups.Where(
                g => g.CourseId == ParseCourseFromMessage(message).Id ).ElementAt(0);
            Deliverable deliverable = new Deliverable(message.Date);
            studentGroup.Deliverables.Add(deliverable);
            this.courseManagmentRepositories.Students.Save();
            // TODO implement
        }

        private Student GetStudentFromMessage(IMessage message)
        {
            // TODO implement
            throw new NotImplementedException();
        }

        private Course ParseCourseFromMessage(IMessage message)
        {
            // TODO implement
            throw new NotImplementedException();
        }
    }
}
