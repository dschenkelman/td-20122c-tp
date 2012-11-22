using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class NewStudentToCourseDatabaseEntryActionFixture
    {
        private MockRepository mockRepository;
        private Mock<IRepository<Student>> studentRepository;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);

            this.logger = this.mockRepository.Create<ILogger>();
		    this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<String>()));

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
            const string MessageAddress = "sebas312@hotmail.com";
            const int StudentId = 90202;
            var student = new Student(StudentId, Name, MessageAddress);
            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses).Verifiable();
            this.courseRepository.Setup(cr => cr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns(student).Verifiable();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IMessage> email = mockRepository.Create<IMessage>();
            email.Setup(e => e.Subject).Returns("[ALTA-MATERIA-" + CourseCode + "] " + StudentId + "-" + Name);
            email.Setup(e => e.From).Returns(MessageAddress);
            email.Setup(e => e.Date).Returns(new DateTime(CourseYear, 9, 5));

            // act
            action.Execute(email.Object, this.logger.Object);

            // assert
            Assert.AreEqual(1, course.Students.Count);
            Assert.AreEqual(StudentId, course.Students.ElementAt(0).Id);
            Assert.AreEqual(Name, course.Students.ElementAt(0).Name);
            Assert.AreEqual(MessageAddress, course.Students.ElementAt(0).MessagingSystemId);
            
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

            Mock<IMessage> email = this.mockRepository.Create<IMessage>();
            const string MessageAddress = "servetto.matias@gmail.com";
            email.Setup(e => e.Subject).Returns("[ALTA-MATERIA-" + CourseCode + "] " + StudentId + "-" + Name);
            email.Setup(e => e.From).Returns(MessageAddress);
            email.Setup(e => e.Date).Returns(new DateTime(CourseYear, 3, 14));

            // act
            action.Execute(email.Object, this.logger.Object);

            // assert
            Assert.AreEqual(1, course.Students.Count);
            Assert.AreEqual(StudentId, course.Students.ElementAt(0).Id);
            Assert.AreEqual(Name, course.Students.ElementAt(0).Name);
            Assert.AreEqual(MessageAddress, course.Students.ElementAt(0).MessagingSystemId);

            this.courseRepository.Verify(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>()), Times.Once());
            this.courseRepository.Verify(cr => cr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.GetById(StudentId), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowWhenAddingAnAlreadyAddedStudentToCourse()
        {
            // arrange
            const int CourseCode = 7508;
            const int CourseYear = 2010;
            Course course = new Course(2, CourseYear, CourseCode);
            const string Name = "Damian Schenkelman";
            const string MessageAddress = "schen.damian@yahoo.com";
            const int StudentId = 90728;
            var student = new Student(StudentId, Name, MessageAddress);
            course.Students.Add(student);
            List<Course> courses = new List<Course> { course };

            this.courseRepository.Setup(cr => cr.Get(It.IsAny<Expression<Func<Course, bool>>>())).Returns(courses).Verifiable();
            this.studentRepository.Setup(sr => sr.GetById(It.IsAny<int>())).Returns(student).Verifiable();

            NewStudentToCourseDatabaseEntryAction action = this.CreateNewStudentToCourseDatabaseEntryAction();

            Mock<IMessage> email = mockRepository.Create<IMessage>();
            email.Setup(e => e.Subject).Returns("[ALTA-MATERIA-" + CourseCode + "] " + StudentId + "-" + Name);
            email.Setup(e => e.From).Returns(MessageAddress);
            email.Setup(e => e.Date).Returns(new DateTime(CourseYear, 11, 19));

            // act
            action.Execute(email.Object, this.logger.Object);
        }

        private NewStudentToCourseDatabaseEntryAction CreateNewStudentToCourseDatabaseEntryAction()
        {
            return new NewStudentToCourseDatabaseEntryAction(this.courseManagementRepositories.Object);
        }
    }
}
