using CourseManagement.Persistence.Configuration;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Utilities.Extensions;

    internal class AddDeliverableToGroupDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;
        private readonly IConfigurationService configurationService;

        public AddDeliverableToGroupDatabaseEntryAction(
            ICourseManagementRepositories courseManagementRepositories,
            IConfigurationService service)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.configurationService = service;
        }

        public void Initialize(ActionEntry actionEntry)
        {
        }

        public void Execute(IMessage message, ILogger logger)
        {
            var student = this.courseManagementRepositories
                .Students
                .Get(s => s.MessagingSystemId == message.From)
                .FirstOrDefault();

            logger.Log(LogLevel.Information, "Obtaining Student's Group");
            Course course = this.GetCourseFromMessage(message);
            var studentGroup = student.Groups.Where(g => g.CourseId == course.Id).FirstOrDefault();
            
            if (studentGroup == null)
            {
                throw new InvalidOperationException("You can not add deliverable to inexistent student's group.");
            }

            logger.Log(LogLevel.Information, "Adding Deliverable to Group");
            Deliverable deliverable = new Deliverable(message.Date);
            deliverable.GroupId = studentGroup.Id;

            string rootPath = this.configurationService.AttachmentsRootPath;
            var directory = Path.Combine(rootPath, message.Subject, message.Date.ToIsoFormat());

            Directory.CreateDirectory(directory);
            foreach (IMessageAttachment messageAttachment in message.Attachments)
            {
                string path = Path.Combine(directory, messageAttachment.Name);
                messageAttachment.Download(path);

                DeliverableAttachment deliverableAttachment = new DeliverableAttachment
                                                                  {
                                                                      FileName = messageAttachment.Name,
                                                                      Location = path
                                                                  }; 
                deliverable.Attachments.Add(deliverableAttachment);
            }

            this.courseManagementRepositories.Deliverables.Insert(deliverable);
            this.courseManagementRepositories.Deliverables.Save();
        }

       private Course GetCourseFromMessage(IMessage message)
        {
            int semester = this.GetSemesterFromMessage(message);

            List<Course> courses =
                this.courseManagementRepositories.Courses.Get(
                    c =>
                    message.To.Contains(c.Account.User) 
                    && c.Semester == semester
                    && c.Year == message.Date.Year).ToList();

            if (courses.Count() == 0)
            {
                // TODO can it really get to this point sometime?
                throw new InvalidOperationException("You can not add deliverable. The account: " + message.To + " is not a valid.");
            }

            return courses.First();
        }

        private int GetSemesterFromMessage(IMessage message)
        {
            return message.Date.Semester();
        }
    }
}
