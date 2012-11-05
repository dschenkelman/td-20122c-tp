namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Services;
    using Utilities.Extensions;

    internal class CreateEmailReplyAction : IAction
    {
        private readonly IMessageSender messageSender;

        protected readonly ICourseManagementRepositories courseManagementRepositories;

        private readonly IConfigurationService configurationService;
        private bool isPublic;
        private string body;
        private string subject;
        private string topicRegex;

        public CreateEmailReplyAction(IMessageSender messageSender, ICourseManagementRepositories courseManagementRepositories, IConfigurationService configurationService)
        {
            this.messageSender = messageSender;
            this.courseManagementRepositories = courseManagementRepositories;
            this.configurationService = configurationService;
        }

        public void Initialize(ActionEntry actionEntry)
        {
            this.isPublic = bool.Parse(actionEntry.AdditionalData["public"]);
            
            if (this.isPublic)
            {
                this.topicRegex = @"^\[CONSULTA-PUBLICA\][\ ]*(?<topic>.*)$";
            }
            else
            {
                this.topicRegex = @"^\[CONSULTA-PRIVADA\][\ ]*(?<topic>.*)$";
            }
            this.body = actionEntry.AdditionalData["body"];
            this.subject = actionEntry.AdditionalData["subject"];
        }

        public void Execute(IMessage message)
        {

            int semester = DateTime.Now.Semester();
            int subjectId = this.configurationService.MonitoredSubjectId;
            Configuration configuration = null;
            List<Course> courses = this.courseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == subjectId &&
                c.Semester == semester).ToList();

            EmailMessage messageToSend = null;

            if (courses.Count > 0)
            {
                messageToSend = this.CreateMessage(message, courses.First());

                this.GetDestinationMessageSystemIds(message, courses.First()).ForEach(dmsid => messageToSend.To.Add(dmsid));

                configuration = courses.First().Account.Configurations.Single(
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
                this.GenerateSubject(receivedMessage) ,
                    course.Account.User,
                    DateTime.Now,
                    new List<EmailAttachment>(),
                    this.body);

            return email;
        }

        protected virtual string GenerateSubject(IMessage receivedMessage)
        {
            return this.subject;
        }
    }
}
