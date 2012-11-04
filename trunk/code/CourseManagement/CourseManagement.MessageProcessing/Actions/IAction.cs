namespace CourseManagement.MessageProcessing.Actions
{
    using Messages;

    public interface IAction
    {
        void Initialize(ActionEntry actionEntry);
        
        void Execute(IMessage message);
    }
}
