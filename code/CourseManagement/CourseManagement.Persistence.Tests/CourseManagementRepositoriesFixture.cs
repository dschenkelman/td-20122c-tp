namespace CourseManagement.Persistence.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Repositories;
    using Moq;

    [TestClass]
    public class CourseManagementRepositoriesFixture
    {
        private Mock<IRepository<DeliverableAttachment>> deliverableAttachments;

        private Mock<IRepository<Course>> courses;

        private Mock<IRepository<Deliverable>> deliverables;

        private Mock<IRepository<Reply>> replies;

        private Mock<IRepository<Subject>> subjects;

        private Mock<IRepository<Student>> students;

        private Mock<IRepository<Teacher>> teachers;

        private Mock<IRepository<Ticket>> tickets;

        private Mock<IRepository<Group>> groups;

        private Mock<IRepository<Account>> accounts;

        private Mock<IRepository<Configuration>> configurations;
        
        private MockRepository mockRepository;
        private Mock<IRepository<TicketAttachment>> ticketAttachments;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.accounts = this.mockRepository.Create<IRepository<Account>>();
            this.deliverableAttachments = this.mockRepository.Create<IRepository<DeliverableAttachment>>();
            this.ticketAttachments = this.mockRepository.Create<IRepository<TicketAttachment>>();
            this.configurations = this.mockRepository.Create<IRepository<Configuration>>();
            this.courses = this.mockRepository.Create<IRepository<Course>>();
            this.deliverables = this.mockRepository.Create<IRepository<Deliverable>>();
            this.groups = this.mockRepository.Create<IRepository<Group>>();
            this.replies = this.mockRepository.Create<IRepository<Reply>>();
            this.students = this.mockRepository.Create<IRepository<Student>>();
            this.subjects = this.mockRepository.Create<IRepository<Subject>>();
            this.teachers = this.mockRepository.Create<IRepository<Teacher>>();
            this.tickets = this.mockRepository.Create<IRepository<Ticket>>();
            
        }

        [TestMethod]
        public void ShouldBeAbleToRetrieveRepositoriesPassedThroughConstructor()
        {
            var repositories = this.CreateRepositories();
            Assert.AreSame(repositories.Accounts, this.accounts.Object);
            Assert.AreSame(repositories.DeliverableAttachments, this.deliverableAttachments.Object);
            Assert.AreSame(repositories.Configurations, this.configurations.Object);
            Assert.AreSame(repositories.Courses, this.courses.Object);
            Assert.AreSame(repositories.Deliverables, this.deliverables.Object);
            Assert.AreSame(repositories.Groups, this.groups.Object);
            Assert.AreSame(repositories.Replies, this.replies.Object);
            Assert.AreSame(repositories.Students, this.students.Object);
            Assert.AreSame(repositories.Subjects, this.subjects.Object);
            Assert.AreSame(repositories.Teachers, this.teachers.Object);
            Assert.AreSame(repositories.Tickets, this.tickets.Object);
            Assert.AreSame(repositories.TicketAttachments, this.ticketAttachments.Object);
        }

        private CourseManagementRepositories CreateRepositories()
        {
            return new CourseManagementRepositories(
                this.accounts.Object,
                this.deliverableAttachments.Object,
                this.ticketAttachments.Object,
                this.configurations.Object,
                this.courses.Object,
                this.deliverables.Object,
                this.groups.Object,
                this.replies.Object,
                this.students.Object,
                this.subjects.Object,
                this.teachers.Object,
                this.tickets.Object);
        }
    }
}
