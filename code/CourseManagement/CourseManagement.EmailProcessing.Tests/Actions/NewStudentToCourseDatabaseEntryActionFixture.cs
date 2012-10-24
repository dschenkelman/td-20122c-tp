using System;
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

            Course course1 = new Course(1, 2, 2012, 7510);
            List<Course> courses = new List<Course> { course1 };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses);
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns(new Student(91363, "Matias Servetto", "servetto.matias@gimail.com"));
            this.studentRepository.Setup(sr => sr.Insert(It.IsAny<Student>()));
            this.studentRepository.Setup(sr => sr.Save()).Verifiable();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();

            // act

            action.Execute(email.Object);

            // assert
            this.courseRepository.Verify(cr => cr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Never());
        }

        [TestMethod]
        public void ShouldAddNoExistingStudentToDictatedCourse()
        {
            // arrange

            Course course1 = new Course(1, 2, 2012, 7510);
            List<Course> courses = new List<Course> { course1 };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses);
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns((Student)null);
            this.studentRepository.Setup(sr => sr.Insert(It.IsAny<Student>()));
            this.studentRepository.Setup(sr => sr.Save()).Verifiable();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();

            // act

            action.Execute(email.Object);

            // assert
            this.courseRepository.Verify(cr => cr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldRetrieveExceptionFromAddingAnAlreadyAddedStudentToCourse()
        {
            // arrange

            Course course1 = new Course(1, 2, 2012, 7510);
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
