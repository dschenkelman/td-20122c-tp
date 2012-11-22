using CourseManagement.Persistence.Configuration;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Utilities.Extensions;

    public class DownloadReplyAttachmentsAction : BaseTicketReplyAction
    {
        private readonly IConfigurationService configurationService;
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public DownloadReplyAttachmentsAction(ICourseManagementRepositories courseManagementRepositories,
            IConfigurationService configurationService) : base()
        {
            this.configurationService = configurationService;
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override void Initialize(ActionEntry actionEntry)
        {
        }

        public override void Execute(IMessage message, ILogger logger)
        {
            int ticketId = this.ParseTicketId(message);
            string rootPath = this.configurationService.AttachmentsRootPath;

            logger.Log(LogLevel.Information, "Obtaining Ticket");

            Ticket ticket = this.courseManagementRepositories.Tickets.GetById(ticketId);

            logger.Log(LogLevel.Information, "Downloading Attachments");
            foreach (var attachment in message.Attachments)
            {
                string name = attachment.Name;
                string downloadDirectoryPath = Path.Combine(rootPath, message.Subject, message.Date.ToIsoFormat());

                Directory.CreateDirectory(downloadDirectoryPath);

                string downloadPath = Path.Combine(downloadDirectoryPath, name);

                attachment.Download(downloadPath);

                TicketAttachment ticketAttachment = new TicketAttachment();
                ticketAttachment.Location = downloadPath;
                ticketAttachment.FileName = name;

                ticket.Attachments.Add(ticketAttachment);
            }

            this.courseManagementRepositories.Tickets.Save();
        }
    }
}