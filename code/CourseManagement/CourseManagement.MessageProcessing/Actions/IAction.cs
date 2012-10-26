namespace CourseManagement.MessageProcessing.Actions
{
    public interface IAction
    {
        void Execute(IMessage message);
    }
}
