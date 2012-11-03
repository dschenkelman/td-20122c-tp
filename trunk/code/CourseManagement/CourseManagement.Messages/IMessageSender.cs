namespace CourseManagement.Messages
{
    public interface IMessageSender
    {
        void Connect(string serverEndpoint, int port, bool useSsl, string user, string password);

        void Send(IMessage message);

        void Disconnect();
    }
}
