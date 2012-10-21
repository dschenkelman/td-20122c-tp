namespace CourseManagement.EmailProcessing.Actions
{
    public interface IAction
    {
        void Execute(IEmail email);
    }
}
