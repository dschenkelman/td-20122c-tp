namespace CourseManagement.Messages
{
    using System.Collections.Generic;

    public interface IMessageReceiver
    {
        void Connect(string serverEndpoint, int port, bool useSsl, string user, string password);

        IEnumerable<IMessage> FetchMessages();

        void Disconnect();
    }
}
