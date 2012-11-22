namespace CourseManagement.MessageProcessing.Actions
{
    using System.IO;
    using Messages;
    using Model;
    using Persistence.Configuration;
    using Persistence.Logging;
    using Persistence.Repositories;
    using Utilities.Extensions;

    public class DownloadReplyAttachmentsAction : BaseTicketReplyAction
    {
        private readonly IConfigurationService configurationService;
        private readonly ILogger logger;
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public DownloadReplyAttachmentsAction(
            ICourseManagementRepositories courseManagementRepositories,
            IConfigurationService configurationService,
            ILogger logger) : base()
        {
            this.configurationService = configurationService;
            this.logger = logger;
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public override void Initialize(ActionEntry actionEntry)
        {
        }

        public override void Execute(IMessage message)
        {
            int ticketId = this.ParseTicketId(message);
            string rootPath = this.configurationService.AttachmentsRootPath;

            this.logger.Log(LogLevel.Information, "Obtaining Ticket");

            Ticket ticket = this.courseManagementRepositories.Tickets.GetById(ticketId);

            this.logger.Log(LogLevel.Information, "Downloading Attachments");
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