namespace CourseManagement.Persistence
{
    using System.Data.Entity;
    using Entities.Model;

    public interface ICourseManagementContext
    {
        IDbSet<Attachment> Attachments { get; }

        IDbSet<Course> Courses { get; }

        IDbSet<Deliverable> Deliverables { get; }

        IDbSet<Group> Groups { get; }

        IDbSet<Reply> Replies { get; }

        IDbSet<Subject> Subjects { get; }

        IDbSet<Student> Students { get; }

        IDbSet<Teacher> Teachers { get; }

        IDbSet<Ticket> Tickets { get; }

        int SaveChanges();

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}