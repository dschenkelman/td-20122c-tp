using System;
using System.Collections.Generic;
using Pop3;

namespace CourseManagement.Messages
{
    public class PopMessageReceiver : IMessageReceiver
    {
        private Pop3MimeClient currentClient;

        public void Connect(string serverEndpoint, int port, bool useSsl, string user, string password)
        {
            if (this.currentClient != null)
            {
                throw new InvalidOperationException("Cannot connect twice using the same receiver. Disconnect first.");
            }

            this.currentClient = new Pop3MimeClient(serverEndpoint, port, useSsl, user, password);

            this.currentClient.Connect();
        }

        public IEnumerable<IMessage> FetchMessages()
        {
            return null;
        }

        public void Disconnect()
        {
            this.currentClient.Disconnect();
            this.currentClient = null;
        }
    }
}
