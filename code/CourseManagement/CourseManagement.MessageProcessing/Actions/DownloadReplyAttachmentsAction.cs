using System;
using System.IO;
using System.Text.RegularExpressions;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Model;
using CourseManagement.Utilities.Extensions;

namespace CourseManagement.MessageProcessing.Actions
{
    using Messages;
    using Persistence.Repositories;

    public class DownloadReplyAttachmentsAction : IAction
    {
        private const string SubjectPattern = @"^\[CONSULTA-(?<ticketId>[0-9]+)\].*$";
        private readonly IConfigurationService configurationService;
        private readonly ICourseManagementRepositories courseManagementRepositories;
        private readonly Regex subjectRegex;

        public DownloadReplyAttachmentsAction(ICourseManagementRepositories courseManagementRepositories,
            IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
            this.courseManagementRepositories = courseManagementRepositories;
            this.subjectRegex = new Regex(SubjectPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public void Initialize(ActionEntry actionEntry)
        {
        }

        public void Execute(IMessage message)
        {
            int ticketId = this.ParseTicketId(message);
            string rootPath = this.configurationService.AttachmentsRootPath;

            Ticket ticket = this.courseManagementRepositories.Tickets.GetById(ticketId);

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

        private int ParseTicketId(IMessage message)
        {
            Match match = this.subjectRegex.Match(message.Subject);
            return Convert.ToInt32(match.Groups["ticketId"].Value);
        }
    }
}