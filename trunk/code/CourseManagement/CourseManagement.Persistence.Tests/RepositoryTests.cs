namespace CourseManagement.Persistence.Tests
{
    using Entities.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RepositoryTests
    {
        private Mock<ICourseManagementContext> context;
        private MockRepository mockRepository;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.context = this.mockRepository.Create<ICourseManagementContext>();
        }

        [TestMethod]
        public void ShouldInvokeContextSaveChangesWhenSaveIsInvoked()
        {
            // arrange
            this.context.Setup(c => c.SaveChanges()).Returns(0).Verifiable();

            var repository = this.CreateRepository();

            // act
            repository.Save();

            // assert
            this.context.Verify(c => c.SaveChanges(), Times.Once());
        }

        private Repository<Subject> CreateRepository()
        {
            return new Repository<Subject>(this.context.Object);
        }
    }
}
