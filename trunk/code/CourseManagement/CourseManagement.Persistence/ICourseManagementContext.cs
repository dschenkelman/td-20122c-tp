namespace CourseManagement.Persistence
{
    using System.Data.Entity;
    using Entities.Model;

    public interface ICourseManagementContext
    {
        DbSet<Attachment> Attachments { get; set; }

        DbSet<Course> Courses { get; set; }

        DbSet<Deliverable> Deliverables { get; set; }

        DbSet<Group> Groups { get; set; }

        DbSet<Reply> Replies { get; set; }

        DbSet<Subject> Subjects { get; set; }

        DbSet<Student> Students { get; set; }

        DbSet<Teacher> Teachers { get; set; }

        DbSet<Ticket> Tickets { get; set; }

        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}