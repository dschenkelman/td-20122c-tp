using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Messages;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using CourseManagement.Utilities.Extensions;

namespace CourseManagement.MessageProcessing.Actions
{
    class ReplyAction : IAction
    {
        private readonly IMessageSender messageSender;
        private readonly ICourseManagementRepositories courseManagementRepositories;
        private readonly IConfigurationService configurationService;
        private bool isPublic;

        public ReplyAction(IMessageSender messageSender, ICourseManagementRepositories courseManagementRepositories, IConfigurationService configurationService)
        {
            this.messageSender = messageSender;
            this.courseManagementRepositories = courseManagementRepositories;
            this.configurationService = configurationService;
            //TODO initialize
            this.isPublic = true;
        }

        public void Initialize(ActionEntry actionEntry)
        {
            throw new NotImplementedException();
        }

        public void Execute(IMessage message)
        {
            EmailMessage messageToSend = CreateMessage(message);

            int semester = DateTime.Now.Semester();
            int subjectId = this.configurationService.MonitoredSubjectId;
            Configuration configuration = null;
            List<Course> courses = this.courseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == subjectId &&
                c.Semester == semester).ToList();

            if (courses.Count > 0)
            {
                this.GetDestinationMessageSystemIds(message, courses.First()).ForEach(dmsid => messageToSend.To.Add(dmsid));

                configuration = courses.First().Account.Configurations.Single(
                    cfg => cfg.Protocol.Equals(this.configurationService.OutgoingMessageProtocol));
            }

            if( configuration == null )
            {
                throw new InvalidOperationException(
                    "You can not send the reply message. There is not outgoing configuration.");
            }

            this.messageSender.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl, configuration.Account.User, configuration.Account.Password);
            this.messageSender.Send( messageToSend );
            this.messageSender.Disconnect();
        }

        private List<string> GetDestinationMessageSystemIds(IMessage message, Course course)
        {
            List<string> destinations = new List<string>{ message.From };
            if(this.isPublic)
            {
                destinations.Add(course.PublicDistributionList);
            }
            return destinations;
        }

        private EmailMessage CreateMessage(IMessage receivedMessage )
        {
            EmailMessage email = new EmailMessage("No reply", "from", DateTime.Now, null, new string[0] );
            return email;
        }
    }
}
