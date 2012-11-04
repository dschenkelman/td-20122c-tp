using System;
using System.IO;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Messages;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.MessageProcessing.Tests.Actions
{
    [TestClass]
    public class AddDeliverableToGroupDatabaseEntryActionFixture
    {
        private MockRepository mockRepository;
        private Mock<IRepository<Student>> studentRepository;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<IRepository<Group>> groupRepository;
        private Mock<IRepository<Deliverable>> deliverableRepository;
        private Mock<IRepository<DeliverableAttachment>> attachmentRepository;
        private Mock<IConfigurationService> configurationService;
        private string rootPath;

        [TestInitialize]
        public void TestInitialize()
        {
            this.rootPath = "c:";
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.groupRepository = this.mockRepository.Create<IRepository<Group>>();
            this.deliverableRepository = this.mockRepository.Create<IRepository<Deliverable>>();
            this.attachmentRepository = this.mockRepository.Create<IRepository<DeliverableAttachment>>();
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.configurationService = this.mockRepository.Create<IConfigurationService>();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Groups).Returns(this.groupRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Deliverables).Returns(this.deliverableRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.DeliverableAttachments).Returns(this.attachmentRepository.Object);
            this.configurationService.Setup(cmr => cmr.RootPath).Returns(rootPath);
        }

        [TestMethod]
        public void ShouldAddDeliverableToGroup()
        {
            // arrange
            Group correctGroup = new Group { Deliverables = new List<Deliverable>(), Id = 1 };
            Student student1CorrectGroup = new Student(91363, "Matias Servetto", "matias.servetto@gmail.com");
            Student student2CorrectGroup = new Student(90202, "Sebastian Rodriguez", "sebastianr213@gmail.com");
            correctGroup.Students = new List<Student> { student1CorrectGroup, student2CorrectGroup };
            Course course = new Course(1, 2012, 7510);
            correctGroup.Course = course;
            Account correctAccount = new Account { User = "ayudantes-7510@gmail.com" };
            course.Account = correctAccount;
            List<Student> students = new List<Student> { student1CorrectGroup };
            student1CorrectGroup.Groups = new List<Group> { correctGroup };

            Course invalidCourse = new Course(2, 2012, 7510);
            invalidCourse.Account = new Account { User = "otro" };
            List<Course> courses = new List<Course> { course };

            this.studentRepository.Setup(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(student1CorrectGroup)))).
                Returns(students).Verifiable();

            this.courseRepository.Setup(
                c =>
                c.Get(
                    It.Is<Expression<Func<Course, bool>>>(
                        f => f.Compile().Invoke(course) && !f.Compile().Invoke(invalidCourse)))).Returns(courses)
                .Verifiable();

            Account incorrectAccount = new Account { User = "teorica-7511@gmail.com" };

            const string DestinationAddress = "ayudantes-7510@gmail.com";
            string sourceAddress = student1CorrectGroup.MessagingSystemId;
            const int NumeroTp = 1;
            string subject = "[ENTREGA-TP-" + NumeroTp + "]";
            
            Mock<IMessageAttachment> messageAttachment = mockRepository.Create<IMessageAttachment>();
            messageAttachment.Setup(e => e.Name).Returns("Menssage");
            messageAttachment.Setup(e => e.Download(Path.Combine(rootPath, subject, "20120201", "Menssage"))).Verifiable();

            DateTime receptionDate = new DateTime(2012, 2, 1);
            Mock<IMessage> message = mockRepository.Create<IMessage>();
            message.Setup(e => e.Subject).Returns(subject);
            message.Setup(e => e.From).Returns(sourceAddress);
            message.Setup(e => e.To).Returns(new List<string> { DestinationAddress });
            message.Setup(e => e.Date).Returns(receptionDate);
            message.Setup(e => e.Attachments).Returns(new List<IMessageAttachment> { messageAttachment.Object });

            this.deliverableRepository.Setup(dr => dr.Insert(
                    It.Is<Deliverable>(d => d.GroupId == correctGroup.Id
                    && d.Attachments.Count == 1
                    && d.Attachments[0].FileName == messageAttachment.Object.Name))).Verifiable();

            this.deliverableRepository.Setup(sr => sr.Save()).Verifiable();

            AddDeliverableToGroupDatabaseEntryAction action = CreateAction();

            // act
            action.Execute(message.Object);
            
            // assert
            this.studentRepository.Verify(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(student1CorrectGroup))),
                Times.Once());

            this.courseRepository.Verify(cr => cr.Get(
                It.Is<Expression<Func<Course, bool>>>(
                    f => f.Compile().Invoke(course) && !f.Compile().Invoke(invalidCourse))), Times.Exactly(1));

            messageAttachment.Verify(e => e.Download(Path.Combine(this.rootPath, subject, "20120201", "Menssage")), Times.Once());

            this.deliverableRepository.Verify(
                    dr => dr.Insert(
                    It.Is<Deliverable>(d => d.GroupId == correctGroup.Id
                    && d.Attachments.Count == 1
                    && d.Attachments[0].FileName == messageAttachment.Object.Name)),
                    Times.Once());

            this.deliverableRepository.Verify(dr => dr.Save(), Times.Once());
        }
        
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotAddDeliverableSendedByANonRegisteredEmail()
        {
            // arrange
            Student trueStudent = new Student(91363, "Matias", "message@address.com");
            Student falseStudent = new Student(90202, "Sebastian", "other.test@address.com");
            this.studentRepository.Setup(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         !f.Compile().Invoke(falseStudent)))).
                Returns(new List<Student>()).Verifiable();

            Mock<IMessage> message = mockRepository.Create<IMessage>();
            message.Setup(e => e.From).Returns("message@address.com");

            AddDeliverableToGroupDatabaseEntryAction action = CreateAction();

            // act
            action.Execute(message.Object);

            // validate
            this.studentRepository.Verify(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         f.Compile().Invoke(falseStudent))),
                Times.Once());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotAddDeliverableSendedByAStudentWithoutAGroup()
        {
            // arrange
            Student trueStudent = new Student(91363, "Matias", "message@address.com");
            Student falseStudent = new Student(90202, "Sebastian", "other.test@address.com");

            Course actualCourse = new Course(1, 2012, 7510) {Id = 5};
            Course previousCourse = new Course(2, 2011, 7510) { Id = 1 };
            Group group = new Group
                              {
                                  Course = previousCourse,
                                  CourseId = previousCourse.Id,
                                  Deliverables = new List<Deliverable>(),
                                  Id = 5,
                                  Students = new List<Student> { trueStudent, falseStudent}
                              };
            trueStudent.Groups = new List<Group> { group };

            this.studentRepository.Setup(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         !f.Compile().Invoke(falseStudent)))).
                Returns(new List<Student>{ trueStudent }).Verifiable();

            actualCourse.Account = new Account() { User = "ayudantes-7510@gmail.com" };
            previousCourse.Account = new Account() { User = "teorica-7511@gmail.com" };

            this.courseRepository.Setup(
                c =>
                c.Get(
                    It.Is<Expression<Func<Course, bool>>>(
                        f => f.Compile().Invoke(actualCourse) && !f.Compile().Invoke(previousCourse)))).Returns(
                            new List<Course> {actualCourse})
                .Verifiable();

            const string DestinationAddress = "ayudantes-7510@gmail.com";
            string sourceAddress = trueStudent.MessagingSystemId;
            const int NumeroTp = 1;
            string subject = "[ENTREGA-TP-" + NumeroTp + "]";
            DateTime receptionDate = new DateTime(2012, 2, 1);
            Mock<IMessage> message = mockRepository.Create<IMessage>();
            message.Setup(e => e.Subject).Returns(subject);
            message.Setup(e => e.From).Returns(sourceAddress);
            message.Setup(e => e.To).Returns(new List<string> {DestinationAddress});
            message.Setup(e => e.Date).Returns(receptionDate);

            AddDeliverableToGroupDatabaseEntryAction action = CreateAction();

            // act
            action.Execute(message.Object);

            // validate
            this.studentRepository.Verify(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         f.Compile().Invoke(falseStudent))),
                Times.Once());

            this.courseRepository.Verify(cr => cr.Get(
                It.Is<Expression<Func<Course, bool>>>(
                    f => f.Compile().Invoke(actualCourse) && !f.Compile().Invoke(previousCourse))), Times.Exactly(2));
        }
        
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotAddDeliverableSendedToInvalidCourseAccount()
        {
            // arrange
            Student trueStudent = new Student(91363, "Matias", "message@address.com");
            Student falseStudent = new Student(90202, "Sebastian", "other.test@address.com");

            Course previousCourse = new Course(2, 2011, 7510) {Id = 1};
            previousCourse.Account = new Account() {User = "teorica@yahoo.com"};
            Group group = new Group
            {
                Course = previousCourse,
                CourseId = previousCourse.Id,
                Deliverables = new List<Deliverable>(),
                Id = 5,
                Students = new List<Student> { trueStudent, falseStudent }
            };
            trueStudent.Groups = new List<Group> { group };

            this.studentRepository.Setup(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         !f.Compile().Invoke(falseStudent)))).
                Returns(new List<Student> { trueStudent }).Verifiable();

            this.courseRepository.Setup(
                c =>
                c.Get(
                    It.Is<Expression<Func<Course, bool>>>(
                        f => !f.Compile().Invoke(previousCourse)))).Returns(
                            new List<Course>())
                .Verifiable();

            const string DestinationAddress = "ayudantes-7510@gmail.com";
            string sourceAddress = trueStudent.MessagingSystemId;
            const int NumeroTp = 1;
            string subject = "[ENTREGA-TP-" + NumeroTp + "]";
            DateTime receptionDate = new DateTime(2012, 2, 1);
            Mock<IMessage> message = mockRepository.Create<IMessage>();
            message.Setup(e => e.Subject).Returns(subject);
            message.Setup(e => e.From).Returns(sourceAddress);
            message.Setup(e => e.To).Returns(new List<string> { DestinationAddress });
            message.Setup(e => e.Date).Returns(receptionDate);

            AddDeliverableToGroupDatabaseEntryAction action = CreateAction();

            // act
            action.Execute(message.Object);

            // validate
            this.studentRepository.Verify(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         f.Compile().Invoke(falseStudent))),
                Times.Once());
        }

        private AddDeliverableToGroupDatabaseEntryAction CreateAction()
        {
            return new AddDeliverableToGroupDatabaseEntryAction(this.courseManagementRepositories.Object,
                                                                this.configurationService.Object);
        }
    }
}
