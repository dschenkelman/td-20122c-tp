namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Services;
    using Utilities.Extensions;

    class AddDeliverableToGroupDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagmentRepositories;
        private readonly IConfigurationService configurationService;

        public AddDeliverableToGroupDatabaseEntryAction(ICourseManagementRepositories courseManagmentRepositories, IConfigurationService service)
        {
            this.courseManagmentRepositories = courseManagmentRepositories;
            this.configurationService = service;
        }

        public void Execute(IMessage message)
        {
            var student = this.courseManagmentRepositories
                .Students
                .Get(s => s.MessagingSystemId == message.From)
                .FirstOrDefault();
            
            if (student == null)
            {
                throw new InvalidOperationException(string.Format("You can not add deliverable to group when student: {0} is not registered", message.From));
            }

            Course course = this.ParseCourseFromMessage(message);
            var studentGroup = student.Groups.Where(g => g.CourseId == course.Id).FirstOrDefault();
            
            if (studentGroup == null)
            {
                throw new InvalidOperationException("You can not add deliverable to inexistent student's group.");
            }

            Deliverable deliverable = new Deliverable(message.Date);
            deliverable.GroupId = studentGroup.Id;

            string rootPath = this.configurationService.RootPath;
            var directory = Path.Combine(rootPath, message.Subject, message.Date.ToIsoFormat());

            Directory.CreateDirectory(directory);
            foreach (IMessageAttachment messageAttachment in message.Attachments)
            {
                string path = Path.Combine(directory, messageAttachment.Name);
                messageAttachment.Download(path);

                DeliverableAttachment deliverableAttachment = new DeliverableAttachment { FileName = messageAttachment.Name, Location = path }; 
                deliverable.Attachments.Add(deliverableAttachment);
            }

            this.courseManagmentRepositories.Deliverables.Insert(deliverable);
            this.courseManagmentRepositories.Deliverables.Save();
        }

       private Course ParseCourseFromMessage(IMessage message)
        {
            int semester = this.GetSemesterFromMessage(message);

            List<Course> courses =
                this.courseManagmentRepositories.Courses.Get(
                    c =>
                    message.To.Contains(c.Account.User) 
                    && c.Semester == semester
                    && c.Year == message.Date.Year).ToList();

            if (courses.Count() == 0)
            {
                throw new InvalidOperationException("You can not add deliverable. The account: " + message.To + " is not a valid.");
            }

            return courses.First();
        }

        private int GetSemesterFromMessage(IMessage message)
        {
            if ((1 <= message.Date.Month) && (message.Date.Month <= 6))
                return 1;
            return 2;
        }
    }
}
