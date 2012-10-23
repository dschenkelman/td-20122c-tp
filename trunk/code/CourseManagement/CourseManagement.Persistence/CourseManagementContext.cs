namespace CourseManagement.Persistence
{
    using System.Data.Entity;
    using Entities.Model;

    public class CourseManagementContext : DbContext, ICourseManagementContext
    {
        public DbSet<Attachment> Attachments { get; set; }
        
        public DbSet<Course> Courses { get; set; }

        public DbSet<Deliverable> Deliverables { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Reply> Replies { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Ticket> Tickets { get; set; }
    }
}
