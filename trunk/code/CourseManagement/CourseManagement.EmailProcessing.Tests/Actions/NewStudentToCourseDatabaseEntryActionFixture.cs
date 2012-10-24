using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using CourseManagement.EmailProcessing.Actions;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.EmailProcessing.Tests.Actions
{
    [TestClass]
    public class NewStudentToCourseDatabaseEntryActionFixture
    {
        private MockRepository mockRepository;
        private Mock<IRepository<Student>> studentRepository;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();
        }

        [TestMethod]
        public void ShouldAddExistingStudentToDictatedCourse()
        {
            // arrange
            Course course = new Course(2, 2012, 7510);
            var student = new Student(91363, "Matias Servetto", "servetto.matias@gimail.com");
            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses);
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns(student);

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();

            // act
            action.Execute(email.Object);

            // assert
            Assert.AreEqual(1, course.Students.Count);
            
            // using linq method ElementAt because ICollection does not have a simpler way to index
            Assert.AreSame(student, course.Students.ElementAt(0));
            this.courseRepository.Verify(cr => cr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Never());
        }

        [TestMethod]
        public void ShouldAddNoExistingStudentToDictatedCourse()
        {
            // arrange
            Course course = new Course(2, 2012, 7510);
            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses).Verifiable();
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(91363)).Returns((Student)null).Verifiable();
            // TODO me pide que mockee este metodo si pongo: It.Is<Student>(s => (s.Id == 91363) && (s.Name == "Matias Serveto"))
            this.studentRepository.Setup(sr => sr.Insert(It.IsAny<Student>())).Verifiable();
            this.studentRepository.Setup(sr => sr.Save()).Verifiable();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();
            email.Setup(e => e.EmailSubject).Returns("[ALTA-MATERIA-CODIGO] 91363-NOMBRE");

            // act
            action.Execute(email.Object);

            // assert
            Assert.AreEqual(1, course.Students.Count);

            this.courseRepository.Verify(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>()), Times.Once());
            this.courseRepository.Verify(cr => cr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.GetById(91363), Times.Once());
            this.studentRepository.Verify(sr => sr.Insert(It.IsAny<Student>()), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldRetrieveExceptionFromAddingAnAlreadyAddedStudentToCourse()
        {
            // arrange

            Course course1 = new Course(2, 2012, 7510);
            Student student = new Student(91363, "Matias Servetto", "servetto.matias@gmail.com");
            course1.Students.Add( student );
            List<Course> courses = new List<Course> { course1 };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses);
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns(student);
            this.studentRepository.Setup(sr => sr.Insert(It.IsAny<Student>()));
            this.studentRepository.Setup(sr => sr.Save()).Verifiable();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();

            // act

            action.Execute(email.Object);
        }

        private NewStudentToCourseDatabaseEntryAction CreateNewStudentToCourseDatabaseEntryAction()
        {
            return new NewStudentToCourseDatabaseEntryAction(this.courseManagementRepositories.Object);
        }
    }
}
