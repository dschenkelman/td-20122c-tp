namespace CourseManagement.Messages
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using Utilities.Extensions;

    public class SmtpMessageSender : IMessageSender
    {
        private SmtpClient currentClient;

        ~SmtpMessageSender()
        {
            if (this.currentClient != null)
            {
                this.currentClient.Dispose();
            }
        }

        public void Connect(string serverEndpoint, int port, bool useSsl, string user, string password)
        {
            if (this.currentClient != null)
            {
                throw new InvalidOperationException("Cannot connect twice using the same sender. Disconnect first.");
            }

            this.currentClient = new SmtpClient(serverEndpoint, port);
            this.currentClient.EnableSsl = useSsl;
            this.currentClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.currentClient.UseDefaultCredentials = false;
            this.currentClient.Credentials = new NetworkCredential(user, password);
        }

        public void Send(IMessage message)
        {
            if (this.currentClient == null)
            {
                throw new InvalidOperationException("Cannot send e-mail if it is not connected.");
            }

            MailMessage emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(message.From);
            message.To.Select(a => new MailAddress(a)).ForEach(emailMessage.To.Add);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = message.Body;

            this.currentClient.Send(emailMessage);
        }

        public void Disconnect()
        {
            this.currentClient.Dispose();
            this.currentClient = null;
        }
    }
}
