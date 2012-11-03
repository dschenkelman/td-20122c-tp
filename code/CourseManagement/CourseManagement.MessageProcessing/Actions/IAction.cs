namespace CourseManagement.MessageProcessing.Actions
{
    using Messages;

    public interface IAction
    {
        void Execute(IMessage message);
    }
}
