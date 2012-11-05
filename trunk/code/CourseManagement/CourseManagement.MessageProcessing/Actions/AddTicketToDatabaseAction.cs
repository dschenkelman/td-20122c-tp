﻿namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.IO;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Services;
    using Utilities.Extensions;

    internal class AddTicketToDatabaseAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagmentRepositories;
        private readonly IConfigurationService configurationService;
        private bool isPrivate;

        public AddTicketToDatabaseAction(ICourseManagementRepositories courseManagmentRepositories, IConfigurationService service)
        {
            this.courseManagmentRepositories = courseManagmentRepositories;
            this.configurationService = service;
        }

        public void Initialize(ActionEntry actionEntry)
        {
            this.isPrivate = !bool.Parse(actionEntry.AdditionalData["public"]);
        }

        public void Execute(IMessage message)
        {
            Ticket ticket = this.ParseTicketFromMessage(message);

            string rootPath = this.configurationService.AttachmentsRootPath;
            var directory = Path.Combine(rootPath, message.Subject, message.Date.ToIsoFormat());

            Directory.CreateDirectory(directory);
            foreach (IMessageAttachment messageAttachment in message.Attachments)
            {
                string path = Path.Combine(directory, messageAttachment.Name);
                messageAttachment.Download(path);

                TicketAttachment attachment = new TicketAttachment { FileName = messageAttachment.Name, Location = path };
                ticket.Attachments.Add(attachment);
            }

            this.courseManagmentRepositories.Tickets.Insert(ticket);
            this.courseManagmentRepositories.Tickets.Save();
        }

        private Ticket ParseTicketFromMessage(IMessage message)
        {
            return new Ticket
                       {
                           StudentId = this.ParseStudentFromMessage(message).Id,
                           DateCreated = message.Date,
                           IsPrivate = this.isPrivate,
                           LastUpdated = message.Date,
                           MessageBody = message.Body,
                           State = TicketState.Unassigned,
                           MessageSubject = message.Subject,
                       };
        }

        private Student ParseStudentFromMessage(IMessage message)
        {
            Student student =
                this.courseManagmentRepositories.Students.Get(s => s.MessagingSystemId == message.From).FirstOrDefault();
            
            if (student == null)
            {
                throw new InvalidOperationException(
                    "You cant create a ticket using an inexistent student's message system id: " + message.From);
            }

            return student;
        }
    }
}
