namespace CourseManagement.Persistence.Repositories
{
    using Model;

    public interface ICourseManagementRepositories
    {
        IRepository<DeliverableAttachment> DeliverableAttachments { get; }

        IRepository<TicketAttachment> TicketAttachments { get; }

        IRepository<Account> Accounts { get; }

        IRepository<Course> Courses { get; }

        IRepository<Configuration> Configurations { get; }

        IRepository<Group> Groups { get; }

        IRepository<Deliverable> Deliverables { get; }

        IRepository<Reply> Replies { get; }

        IRepository<Subject> Subjects { get; }

        IRepository<Student> Students { get; }

        IRepository<Teacher> Teachers { get; }

        IRepository<Ticket> Tickets { get; }
    }
}