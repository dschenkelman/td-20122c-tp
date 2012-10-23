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
        
        IDbSet<TEntity> ICourseManagementContext.Set<TEntity>()
        {
            return this.Set<TEntity>();
        }

        IDbSet<Attachment> ICourseManagementContext.Attachments
        {
            get { return this.Attachments; }
        }

        IDbSet<Course> ICourseManagementContext.Courses
        {
            get { return this.Courses; }
        }

        IDbSet<Deliverable> ICourseManagementContext.Deliverables
        {
            get { return this.Deliverables; }
        }

        IDbSet<Group> ICourseManagementContext.Groups
        {
            get { return this.Groups; }
        }

        IDbSet<Reply> ICourseManagementContext.Replies
        {
            get { return this.Replies; }
        }

        IDbSet<Subject> ICourseManagementContext.Subjects
        {
            get { return this.Subjects; }
        }

        IDbSet<Student> ICourseManagementContext.Students
        {
            get { return this.Students; }
        }

        IDbSet<Teacher> ICourseManagementContext.Teachers
        {
            get { return this.Teachers; }
        }

        IDbSet<Ticket> ICourseManagementContext.Tickets
        {
            get { return this.Tickets; }
        }
    }
}
