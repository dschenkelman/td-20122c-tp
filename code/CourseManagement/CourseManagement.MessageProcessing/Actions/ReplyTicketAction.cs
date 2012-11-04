using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Messages;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing.Actions
{
    class ReplyTicketAction : ReplyAction
    {
        public ReplyTicketAction(IMessageSender messageSender, ICourseManagementRepositories courseManagementRepositories, IConfigurationService configurationService) : base(messageSender, courseManagementRepositories, configurationService)
        {
        }

        protected new virtual string GenerateSubject(IMessage message)
        {
            return "[CONSULTA-" +
                   this.courseManagementRepositories.Tickets.Get(t => t.MessageSubject == message.Subject).ToList().
                       First().Id + "] Creada";
        }
    }
}
