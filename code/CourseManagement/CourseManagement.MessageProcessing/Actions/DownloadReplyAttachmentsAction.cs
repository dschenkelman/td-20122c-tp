namespace CourseManagement.MessageProcessing.Actions
{
    using Messages;
    using Persistence.Repositories;

    public class DownloadReplyAttachmentsAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public DownloadReplyAttachmentsAction(ICourseManagementRepositories courseManagementRepositories)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public void Initialize(ActionEntry actionEntry)
        {
        }

        public void Execute(IMessage message)
        {
        }
    }
}
