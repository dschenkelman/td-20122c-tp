using System;

namespace CourseManagement.Persistence.Repositories
{
    using Model;

    public class CourseManagementRepositories : ICourseManagementRepositories
    {
        private readonly IRepository<DeliverableAttachment> deliverableAttachments;
        private readonly IRepository<TicketAttachment> ticketAttachments;
        private readonly IRepository<Account> accounts;
        private readonly IRepository<Configuration> configurations;
        private readonly IRepository<Course> courses;
        private readonly IRepository<Deliverable> deliverables;
        private readonly IRepository<Group> groups;
        private readonly IRepository<Reply> replies;
        private readonly IRepository<Student> students;
        private readonly IRepository<Subject> subjects;
        private readonly IRepository<Teacher> teachers;
        private readonly IRepository<Ticket> tickets;
        

        public CourseManagementRepositories(IRepository<Account> accounts,
            IRepository<DeliverableAttachment> deliverableAttachments,
            IRepository<TicketAttachment> ticketAttachments,
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
            this.deliverableAttachments = deliverableAttachments;
            this.ticketAttachments = ticketAttachments;
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

        public IRepository<TicketAttachment> TicketAttachments
        {
            get { return this.ticketAttachments; }
        }

        public IRepository<Account> Accounts
        {
            get { return this.accounts; }
        }
        
        public IRepository<DeliverableAttachment> DeliverableAttachments
        {
            get { return this.deliverableAttachments; }
        }

        public IRepository<Configuration> Configurations
        {
            get { return this.configurations; }
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
    }
}
