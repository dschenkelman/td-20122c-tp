namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;

    class AddDeliverableToGroupDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagmentRepositories;

        public AddDeliverableToGroupDatabaseEntryAction(ICourseManagementRepositories courseManagmentRepositories)
        {
            this.courseManagmentRepositories = courseManagmentRepositories;
        }

        public void Execute(IMessage message)
        {
            var students = this.courseManagmentRepositories.Students.Get(s => s.MessagingSystemId == message.From);
            if (students.Count() == 0)
            {
                throw new Exception("There is no student registered with: " + message.From);
            }

            var studentGroups = students.ElementAt(0).Groups.Where(g => g.CourseId == this.ParseCourseFromMessage(message).Id);
            if (studentGroups.Count() == 0)
            {
                throw new Exception("The student has not been assigned to a group for the course");
            }

            Deliverable deliverable = new Deliverable(message.Date);
            deliverable.Attachments = new List<Attachment>();
            studentGroups.ElementAt(0).Deliverables.Add(deliverable);
            //foreach( string attachmentPath in message.AttachmentPaths )
            //{
            //    Attachment attachment = new Attachment();
            //    attachment.FileName = attachmentPath.Substring(attachmentPath.LastIndexOf("\\") + 1);
            //    attachment.Location = attachmentPath.Substring(0, attachmentPath.LastIndexOf("\\"));
            //    deliverable.Attachments.Add(attachment);

            //    courseManagmentRepositories.Attachments.Insert(attachment);
            //}
            
            this.courseManagmentRepositories.Deliverables.Insert(deliverable);

            this.courseManagmentRepositories.Deliverables.Save();
            this.courseManagmentRepositories.Groups.Save();
            this.courseManagmentRepositories.Students.Save();
        }

        private Course ParseCourseFromMessage(IMessage message)
        {
            int subjectId = GetSubjectIdFromMessage(message);
            int semester = GetSemesterFromMessage(message);
            List<Course> courses = courseManagmentRepositories.Courses.Get(
                c =>
                c.Semester == semester && c.Year == message.Date.Year && c.SubjectId == subjectId).ToList();
            if( courses.Count() == 0 )
            {
                throw new Exception("Can not find the specific course");
            }
            return courses.ElementAt(0);
        }

        private int GetSubjectIdFromMessage(IMessage message)
        {
            List<Account> accounts = courseManagmentRepositories.Accounts.Get(a => a.User == message.To).ToList();
            if( accounts.Count() == 0 )
            {
                throw new Exception( "The account: " + message.To + " is not a valid." );
            }
            return accounts.ElementAt(0).Course.SubjectId;
        }

        private int GetSemesterFromMessage(IMessage message)
        {
            if ((1 <= message.Date.Month) && (message.Date.Month <= 6))
                return 1;
            return 2;
        }
    }
}
