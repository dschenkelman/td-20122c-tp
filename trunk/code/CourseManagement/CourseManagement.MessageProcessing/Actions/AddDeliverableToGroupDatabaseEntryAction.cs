﻿using System;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Messages;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using System.IO;

namespace CourseManagement.MessageProcessing.Actions
{
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
            var students = courseManagmentRepositories.Students.Get( s => s.MessagingSystemId == message.From );
            if( students.Count() == 0 )
            {
                throw new InvalidOperationException("You can not add deliverable to group when student: " + message.From + " is not registered");
            }
            Course course = ParseCourseFromMessage(message);
            var studentGroups = students.ElementAt(0).Groups.Where(
                g => g.CourseId == course.Id);
            if( studentGroups.Count() == 0 )
            {
                throw new InvalidOperationException("You can not add deliverable to inexistent student's group.");
            }
            Deliverable deliverable = new Deliverable(message.Date) {Attachments = new List<Attachment>()};
            studentGroups.ElementAt(0).Deliverables.Add(deliverable);

            string rootPath = configurationService.RootPath;
            var directory = Path.Combine(rootPath, message.Subject, DateToYYYYMMDD( message.Date ));

            Directory.CreateDirectory(directory);
            foreach (IMessageAttachment messageAttachment in message.Attachments)
            {
                string path = Path.Combine(directory, messageAttachment.Name);
                messageAttachment.Download(path);

                Attachment attachment = new Attachment { FileName = messageAttachment.Name, Location = path }; 
                deliverable.Attachments.Add(attachment);

                courseManagmentRepositories.Attachments.Insert(attachment);
            }
            courseManagmentRepositories.Deliverables.Insert(deliverable);

            courseManagmentRepositories.Deliverables.Save();
            courseManagmentRepositories.Groups.Save();
            courseManagmentRepositories.Students.Save();
        }

        private string DateToYYYYMMDD(DateTime date)
        {
            string year = date.Year + "";
            string month = date.Month + "";
            string day = date.Day + "";

            if( day.Length == 1 )
            {
                day = "0" + day;
            }
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            return year + month + day;
        }

        private Course ParseCourseFromMessage(IMessage message)
        {
            List<Course> courses =
                courseManagmentRepositories.Courses.Get(
                    c =>
                    message.To.Contains(c.Account.User) && c.Semester == GetSemesterFromMessage(message) && c.Year == message.Date.Year 
                   ).ToList();

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