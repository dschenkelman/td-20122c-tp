namespace CourseManagement.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pop3;

    public class PopMessageReceiver : IMessageReceiver
    {
        private Pop3MimeClient currentClient;

        private const int TimeoutInMilliseconds = 60000;

        public void Connect(string serverEndpoint, int port, bool useSsl, string user, string password)
        {
            if (this.currentClient != null)
            {
                throw new InvalidOperationException("Cannot connect twice using the same receiver. Disconnect first.");
            }

            this.currentClient = new Pop3MimeClient(serverEndpoint, port, useSsl, user, password);

            this.currentClient.Connect();

            this.currentClient.ReadTimeout = TimeoutInMilliseconds;
        }

        public IEnumerable<IMessage> FetchMessages()
        {
            int inboxEmails;
            int mailboxSize;
            this.currentClient.GetMailboxStats(out inboxEmails, out mailboxSize);

            RxMailMessage message;

            for (int i = 1; i <= inboxEmails; i++)
            {
                this.currentClient.GetEmail(i, out message);

                yield return new EmailMessage(
                                                message.Subject,
                                                message.From.Address,
                                                message.DeliveredTo.Address,
                                                message.DeliveryDate,
                                                message.Attachments.Select(a => new EmailAttachment(a.Name, () => a.ContentStream)));
            }
        }

        public void Disconnect()
        {
            this.currentClient.Disconnect();
            this.currentClient = null;
        }
    }
}