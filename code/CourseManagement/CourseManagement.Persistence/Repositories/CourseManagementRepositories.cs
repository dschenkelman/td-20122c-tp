namespace CourseManagement.Persistence.Repositories
{
    using Model;

    public class CourseManagementRepositories : ICourseManagementRepositories
    {
        private readonly IRepository<Attachment> attachments;
        private readonly IRepository<Course> courses;
        private readonly IRepository<Deliverable> deliverables;
        private readonly IRepository<Group> groups;
        private readonly IRepository<Reply> replies;
        private readonly IRepository<Student> students;
        private readonly IRepository<Subject> subjects;
        private readonly IRepository<Teacher> teachers;
        private readonly IRepository<Ticket> tickets;
        private readonly IRepository<Account> accounts;
        private readonly IRepository<Configuration> configurations;

        public CourseManagementRepositories(IRepository<Account> accounts,
            IRepository<Attachment> attachments,
            IRepository<Configuration> configurations,
            IRepository<Course> courses, 
            IRepository<Deliverable> deliverables,
            IRepository<Group> groups, 
            IRepository<Reply> replies,
            IRepository<Student> students,
            IRepository<Subject> subjects,
            IRepository<Teacher> teachers,
            IRepository<Ticket> tickets)
        {
            this.attachments = attachments;
            this.courses = courses;
            this.deliverables = deliverables;
            this.groups = groups;
            this.replies = replies;
            this.students = students;
            this.subjects = subjects;
            this.teachers = teachers;
            this.tickets = tickets;
            this.accounts = accounts;
            this.configurations = configurations;
        }

        public IRepository<Attachment> Attachments
        {
            get { return this.attachments; }
        }

        public IRepository<Course> Courses
        {
            get { return this.courses; }
        }

        public IRepository<Deliverable> Deliverables
        {
            get { return this.deliverables; }
        }

        public IRepository<Reply> Replies
        {
            get { return this.replies; }
        }

        public IRepository<Subject> Subjects
        {
            get { return this.subjects; }
        }

        public IRepository<Student> Students
        {
            get { return this.students; }
        }

        public IRepository<Teacher> Teachers
        {
            get { return this.teachers; }
        }

        public IRepository<Ticket> Tickets
        {
            get { return this.tickets; }
        }

        public IRepository<Group> Groups
        {
            get { return this.groups; }
        }

        public IRepository<Account> Accounts
        {
            get { return this.accounts; }
        }

        public IRepository<Configuration> Configurations
        {
            get { return this.configurations; }
        }
    }
}
