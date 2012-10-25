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

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
        }

        [TestMethod]
        public void ShouldAddExistingStudentToDictatedCourse()
        {
            // arrange
            const int CourseCode = 7510;
            const int CourseYear = 2012;
            const int CourseSemester = 2;
            Course course = new Course(CourseSemester, CourseYear, CourseCode);
            const string Name = "Sebastian Rodriguez";
            const string EmailAddress = "sebas312@hotmail.com";
            const int StudentId = 90202;
            var student = new Student(StudentId, Name, EmailAddress);
            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses).Verifiable();
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns(student).Verifiable();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();
            email.Setup(e => e.EmailSubject).Returns("[ALTA-MATERIA-" + CourseCode + "] " + StudentId + "-" + Name);
            email.Setup(e => e.Address).Returns(EmailAddress);
            email.Setup(e => e.Date).Returns(new DateTime(CourseYear, 9, 5));

            // act
            action.Execute(email.Object);

            // assert
            Assert.AreEqual(1, course.Students.Count);
            Assert.AreEqual(StudentId, course.Students.ElementAt(0).Id);
            Assert.AreEqual(Name, course.Students.ElementAt(0).Name);
            Assert.AreEqual(EmailAddress, course.Students.ElementAt(0).EmailAdress);
            
            // using linq method ElementAt because ICollection does not have a simpler way to index
            Assert.AreSame(student, course.Students.ElementAt(0));
            this.courseRepository.Verify(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>()), Times.Once());
            this.studentRepository.Verify(sr => sr.GetById(It.IsAny<int>()), Times.Once());

            this.courseRepository.Verify(cr => cr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Never());
        }

        [TestMethod]
        public void ShouldAddNoExistingStudentToDictatedCourse()
        {
            // arrange
            const int CourseCode = 7509;
            const int CourseYear = 2011;
            const int StudentId = 91363;
            const string Name = "Matias";
            Course course = new Course(1, CourseYear, CourseCode);
            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses).Verifiable();
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(StudentId)).Returns((Student)null).Verifiable();
            this.studentRepository.Setup(sr => sr.Insert(It.Is<Student>(s => s.Id == StudentId && s.Name == Name))).Verifiable();
            this.studentRepository.Setup(sr => sr.Save()).Verifiable();

            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();
            const string EmailAddress = "servetto.matias@gmail.com";
            email.Setup(e => e.EmailSubject).Returns("[ALTA-MATERIA-" + CourseCode + "] " + StudentId + "-" + Name);
            email.Setup(e => e.Address).Returns(EmailAddress);
            email.Setup(e => e.Date).Returns(new DateTime(CourseYear, 3, 14));

            // act
            action.Execute(email.Object);

            // assert
            Assert.AreEqual(1, course.Students.Count);
            Assert.AreEqual(StudentId, course.Students.ElementAt(0).Id);
            Assert.AreEqual(Name, course.Students.ElementAt(0).Name);
            Assert.AreEqual(EmailAddress, course.Students.ElementAt(0).EmailAdress);

            this.courseRepository.Verify(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>()), Times.Once());
            this.courseRepository.Verify(cr => cr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.GetById(StudentId), Times.Once());
            this.studentRepository.Verify(sr => sr.Insert(It.Is<Student>(s => s.Id == StudentId && s.Name == Name)), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldRetrieveExceptionFromAddingAnAlreadyAddedStudentToCourse()
        {
            // arrange
            const int CourseCode = 7508;
            const int CourseYear = 2010;
            Course course = new Course(2, CourseYear, CourseCode);
            const string Name = "Damian Schenkelman";
            const string EmailAddress = "schen.damian@yahoo.com";
            const int StudentId = 90728;
            var student = new Student(StudentId, Name, EmailAddress);
            course.Students.Add( student );
            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns(student).Verifiable();

            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();
            email.Setup(e => e.EmailSubject).Returns("[ALTA-MATERIA-" + CourseCode + "] " + StudentId + "-" + Name);
            email.Setup(e => e.Address).Returns(EmailAddress);
            email.Setup(e => e.Date).Returns(new DateTime(CourseYear, 11, 19));

            // act
            action.Execute(email.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldRetrieveExceptionFromTryingToSubscribeToAnInexistentCourse()
        {
            // arrange
            List<Course> courses = new List<Course>();
            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses).Verifiable();

            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();
            const string Name = "Damian Schenkelman";
            const int StudentId = 90728;
            const int CourseCode = 7508;
            email.Setup(e => e.EmailSubject).Returns("[ALTA-MATERIA-" + CourseCode + "] " + StudentId + "-" + Name);
            const string EmailAddress = "schen.damian@yahoo.com";
            email.Setup(e => e.Address).Returns(EmailAddress);
            email.Setup(e => e.Date).Returns(new DateTime(2010, 11, 19));

            // act
            action.Execute(email.Object);
        }

        private NewStudentToCourseDatabaseEntryAction CreateNewStudentToCourseDatabaseEntryAction()
        {
            return new NewStudentToCourseDatabaseEntryAction(this.courseManagementRepositories.Object);
        }
    }
}
