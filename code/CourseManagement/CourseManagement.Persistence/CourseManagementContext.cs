using CourseManagement.Model;

namespace CourseManagement.Persistence
{
    using System.Data.Entity;

    public class CourseManagementContext : DbContext, ICourseManagementContext
    {
        public IDbSet<DeliverableAttachment> Attachments { get; set; }
        
        public IDbSet<Course> Courses { get; set; }

        public IDbSet<Deliverable> Deliverables { get; set; }

        public IDbSet<Group> Groups { get; set; }

        public IDbSet<Reply> Replies { get; set; }

        public IDbSet<Subject> Subjects { get; set; }

        public IDbSet<Student> Students { get; set; }

        public IDbSet<Teacher> Teachers { get; set; }

        public IDbSet<Ticket> Tickets { get; set; }

        IDbSet<TEntity> ICourseManagementContext.Set<TEntity>()
        {
            return this.Set<TEntity>();
        }
    }
}
