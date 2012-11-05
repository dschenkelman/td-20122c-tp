using CourseManagement.MessageProcessing.Services;

namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;

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
            this.isPrivate = !Boolean.Parse(actionEntry.AdditionalData["public"]);
        }

        public void Execute(IMessage message)
        {
            Ticket ticket = ParseTicketFromMessage(message);

            if( this.courseManagmentRepositories.Tickets.Get(t => t.MessageSubject == message.Subject).ToList().Count() != 0)
            {
                throw new InvalidOperationException("You can not add a new ticket with the same topic (" +
                                                    ParseTopicFromMessage(message) + ") as a previous existing ticket.");
            }

            string rootPath = configurationService.RootPath;
            var directory = Path.Combine(rootPath, message.Subject, DateToYYYYMMDD( message.Date ));

            Directory.CreateDirectory(directory);
            foreach (IMessageAttachment messageAttachment in message.Attachments)
            {
                string path = Path.Combine(directory, messageAttachment.Name);
                messageAttachment.Download(path);

                TicketAttachment attachment = new TicketAttachment() { FileName = messageAttachment.Name, Location = path };
                ticket.Attachments.Add(attachment);

                this.courseManagmentRepositories.TicketAttachments.Insert(attachment);
            }

            this.courseManagmentRepositories.TicketAttachments.Save();

            this.courseManagmentRepositories.Tickets.Insert(ticket);
            this.courseManagmentRepositories.Tickets.Save();
        }

        private Ticket ParseTicketFromMessage(IMessage message)
        {
            return new Ticket
                       {
                           AssignedTeacher = null,
                           Creator = this.ParseStudentFromMessage(message),
                           DateCreated = message.Date,
                           IsPrivate = this.isPrivate,
                           LastUpdated = message.Date,
                           MessageBody = message.Body,
                           State = TicketState.Unassigned,
                           MessageSubject = message.Subject,
                           Replies = new List<Reply>()
                       };
        }

        private Student ParseStudentFromMessage(IMessage message)
        {
            List<Student> students =
                this.courseManagmentRepositories.Students.Get(s => s.MessagingSystemId == message.From).ToList();
            if( students.Count() == 0 )
            {
                throw new InvalidOperationException(
                    "You cant create a ticket using an inexistent student's message system id: " + message.From);
            }
            return students.First();
        }

        private string ParseTopicFromMessage(IMessage message)
        {
            return message.Subject.Split(']')[1];
        }

        private string DateToYYYYMMDD(DateTime date)
        {
            string year = date.Year + "";
            string month = date.Month + "";
            string day = date.Day + "";

            if (day.Length == 1)
            {
                day = "0" + day;
            }
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            return year + month + day;
        }
    }
}
