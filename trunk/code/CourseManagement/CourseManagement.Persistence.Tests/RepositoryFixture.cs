namespace CourseManagement.Persistence.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Repositories;

    [TestClass]
    public class RepositoryFixture
    {
        private Mock<ICourseManagementContext> context;
        private MockRepository mockRepository;
        private Mock<IDbSet<Subject>> subjects;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.context = this.mockRepository.Create<ICourseManagementContext>();
            this.subjects = this.mockRepository.Create<IDbSet<Subject>>();

            this.context.Setup(c => c.Set<Subject>()).Returns(this.subjects.Object);
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

        [TestMethod]
        public void ShouldFindSubjectWithIdWhenRetrievingSubjectById()
        {
            // arrange
            const int SubjectIdToFind = 3;
            const string SubjectName = "Software Design";

            Subject subject = new Subject { Code = SubjectIdToFind, Name = SubjectName };

            this.subjects.Setup(s => s.Find(SubjectIdToFind)).Returns(subject).Verifiable();

            var repository = this.CreateRepository();

            // act
            var retrievedSubject = repository.GetById(SubjectIdToFind);

            // assert
            Assert.AreSame(retrievedSubject, subject);
            Assert.AreEqual(SubjectName, retrievedSubject.Name);
            this.subjects.Verify(s => s.Find(SubjectIdToFind), Times.Once());
        }

        [TestMethod]
        public void ShouldReturnSubjectsThatMatchFilterWhenRetrievingSubjects()
        {
            // arrange
            Subject subject1 = new Subject { Code = 1 };
            Subject subject2 = new Subject { Code = 2 };
            Subject subject3 = new Subject { Code = 3 };

            var subjectsToReturn = new List<Subject> { subject1, subject2, subject3 };

            var queryableSubject = subjectsToReturn.AsQueryable();

            this.subjects.Setup(s => s.Expression).Returns(queryableSubject.Expression);
            this.subjects.Setup(s => s.Provider).Returns(queryableSubject.Provider).Verifiable();

            var repository = this.CreateRepository();

            // act
            var retrievedSubjects = repository.Get(s => s.Code % 2 != 0).ToList();

            // assert
            Assert.AreEqual(2, retrievedSubjects.Count);
            Assert.AreSame(subject1, retrievedSubjects[0]);
            Assert.AreSame(subject3, retrievedSubjects[1]);
        }

        [TestMethod]
        public void ShouldCallAddOnSetWhenInsertingSubject()
        {
            // arrange
            Subject subject1 = new Subject { Code = 1 };
            Subject subject2 = new Subject { Code = 2 };

            var repository = this.CreateRepository();

            this.subjects.Setup(s => s.Add(subject1)).Returns(subject1).Verifiable();
            this.subjects.Setup(s => s.Add(subject2)).Returns(subject2).Verifiable();

            // act
            repository.Insert(subject1);
            repository.Insert(subject2);

            // assert);
            this.subjects.Verify(s => s.Add(subject1), Times.Once());
            this.subjects.Verify(s => s.Add(subject2), Times.Once());
        }

        [TestMethod]
        public void ShouldFindSubjectByIdAndThenRemoveWhenDeletingSubjectById()
        {
            // arrange
            const int CodeToDelete = 1;
            Subject subject = new Subject { Code = CodeToDelete };

            var repository = this.CreateRepository();

            this.subjects.Setup(s => s.Find(CodeToDelete)).Returns(subject).Verifiable();
            this.subjects.Setup(s => s.Remove(subject)).Returns(subject).Verifiable();

            // act
            repository.Delete(CodeToDelete);
            
            // assert;
            this.subjects.Verify(s => s.Find(CodeToDelete), Times.Once());
            this.subjects.Verify(s => s.Remove(subject), Times.Once());
        }

        private Repository<Subject> CreateRepository()
        {
            return new Repository<Subject>(this.context.Object);
        }
    }
}
