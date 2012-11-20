using CourseManagement.Persistence.Configuration;

namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Utilities.Extensions;

    public class CreateEmailReplyAction : IAction
    {
        protected readonly ICourseManagementRepositories CourseManagementRepositories;

        private readonly IMessageSender messageSender;
        private readonly IConfigurationService configurationService;
        private bool isPublic;
        private string body;
        private string subject;

        public CreateEmailReplyAction(IMessageSender messageSender, ICourseManagementRepositories courseManagementRepositories, IConfigurationService configurationService)
        {
            this.messageSender = messageSender;
            this.CourseManagementRepositories = courseManagementRepositories;
            this.configurationService = configurationService;
        }

        public void Initialize(ActionEntry actionEntry)
        {
            this.isPublic = bool.Parse(actionEntry.AdditionalData["public"]);
            this.body = actionEntry.AdditionalData["body"];
            this.subject = actionEntry.AdditionalData["subject"];
        }

        public void Execute(IMessage message)
        {
            int semester = DateTime.Now.Semester();
            int subjectId = this.configurationService.MonitoredSubjectId;
            Configuration configuration = null;
            Course course = this.CourseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == subjectId &&
                c.Semester == semester).FirstOrDefault();

            EmailMessage messageToSend = null;

            if (course != null)
            {
                messageToSend = this.CreateMessage(message, course);

                this.GetDestinationMessageSystemIds(message, course).ForEach(dmsid => messageToSend.To.Add(dmsid));

                configuration = course.Account.Configurations.First(
                    cfg => cfg.Protocol.Equals(this.configurationService.OutgoingMessageProtocol));
            }

            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "You can not send the reply message. There is not outgoing configuration.");
            }

            this.messageSender.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl, configuration.Account.User, configuration.Account.Password);
            this.messageSender.Send(messageToSend);
            this.messageSender.Disconnect();
        }

        protected virtual string GenerateSubject(IMessage receivedMessage)
        {
            return this.subject;
        }

        private List<string> GetDestinationMessageSystemIds(IMessage message, Course course)
        {
            List<string> destinations = new List<string> { message.From };
            if (this.isPublic)
            {
                destinations.Add(course.PublicDistributionList);
            }

            return destinations;
        }

        private EmailMessage CreateMessage(IMessage receivedMessage, Course course)
        {
            EmailMessage email = new EmailMessage(
                this.GenerateSubject(receivedMessage),
                    course.Account.User,
                    DateTime.Now,
                    new List<EmailAttachment>(),
                    this.body);

            return email;
        }
    }
}
