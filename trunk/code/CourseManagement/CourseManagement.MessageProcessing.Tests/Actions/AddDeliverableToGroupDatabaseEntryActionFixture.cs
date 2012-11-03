﻿using System;
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
        private Mock<IRepository<Account>> accountRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<IRepository<Group>> groupRepository;
        private Mock<IRepository<Deliverable>> deliverableRepository;
        private Mock<IRepository<Attachment>> attachmentRepository;
        private Mock<IConfigurationService> configurationService;
        private string RootPath;

        [TestInitialize]
        public void TestInitialize()
        {
            this.RootPath = "c:";
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.groupRepository = this.mockRepository.Create<IRepository<Group>>();
            this.deliverableRepository = this.mockRepository.Create<IRepository<Deliverable>>();
            this.accountRepository = this.mockRepository.Create<IRepository<Account>>();
            this.attachmentRepository = this.mockRepository.Create<IRepository<Attachment>>();
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.configurationService = this.mockRepository.Create<IConfigurationService>();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Groups).Returns(this.groupRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Deliverables).Returns(this.deliverableRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Accounts).Returns(this.accountRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Attachments).Returns(this.attachmentRepository.Object);
            this.configurationService.Setup(cmr => cmr.RootPath).Returns(RootPath);
        }

        [TestMethod]
        public void ShouldAddDeliverableToGroup()
        {
            // arrange
            Group correctGroup = new Group {Deliverables = new List<Deliverable>(), Id = 1};
            Student student1CorrectGroup = new Student(91363, "Matias Servetto", "matias.servetto@gmail.com");
            Student student2CorrectGroup = new Student(90202, "Sebastian Rodriguez", "sebastianr213@gmail.com");
            correctGroup.Students = new List<Student> {student1CorrectGroup, student2CorrectGroup};
            Course course = new Course(1,2012,7510);
            correctGroup.Course = course;
            List<Student> students = new List<Student>{ student1CorrectGroup };
            student1CorrectGroup.Groups = new List<Group> {correctGroup};

            Course invalidCourse = new Course(2,2012,7510);
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
            Account correctAccount = new Account {User = "ayudantes-7510@gmail.com", Course = course};
            Account incorrectAccount = new Account { User = "teorica-7511@gmail.com" };
            this.accountRepository.Setup(
                ar =>
                ar.Get(
                    It.Is<Expression<Func<Account, bool>>>(
                        f => f.Compile().Invoke(correctAccount) && !f.Compile().Invoke(incorrectAccount)))).
                Returns(new List<Account> {correctAccount}).Verifiable();

            this.attachmentRepository.Setup(ar => ar.Insert(It.IsAny<Attachment>())).Verifiable();
            this.deliverableRepository.Setup(ar => ar.Insert(It.IsAny<Deliverable>())).Verifiable();

            this.deliverableRepository.Setup(sr => sr.Save()).Verifiable();
            this.groupRepository.Setup(sr => sr.Save()).Verifiable();
            this.studentRepository.Setup(sr => sr.Save()).Verifiable();

            Mock<IMessageAttachment> messageAttachment = mockRepository.Create<IMessageAttachment>();
            messageAttachment.Setup(e => e.Name).Returns("Menssage");

            const string DestinationAddress = "ayudantes-7510@gmail.com";
            string sourceAddress = student1CorrectGroup.MessagingSystemId;
            const int NumeroTp = 1;
            string subject = "[ENTREGA-TP-" + NumeroTp + "]";
            DateTime receptionDate = new DateTime(2012, 2, 1);
            Mock<IMessage> message = mockRepository.Create<IMessage>();
            message.Setup(e => e.Subject).Returns(subject);
            message.Setup(e => e.From).Returns(sourceAddress);
            message.Setup(e => e.To).Returns(new List<string> {DestinationAddress});
            message.Setup(e => e.Date).Returns(receptionDate);
            message.Setup(e => e.Attachments).Returns(new List<IMessageAttachment> { messageAttachment.Object });

            messageAttachment.Setup(e => e.Download(Path.Combine(RootPath, subject, "20120201", "Menssage"))).Verifiable();

            AddDeliverableToGroupDatabaseEntryAction action = CreateAction();

            // act
            action.Execute(message.Object);
            
            // assert
            Assert.AreEqual(1, correctGroup.Deliverables.Count());
            Assert.AreEqual(receptionDate, correctGroup.Deliverables.ElementAt(0).ReceptionDate);

            this.studentRepository.Verify(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(student1CorrectGroup))),
                Times.Once());
            this.courseRepository.Verify(cr => cr.Get(
                It.Is<Expression<Func<Course, bool>>>(
                    f => f.Compile().Invoke(course) && !f.Compile().Invoke(invalidCourse))), Times.Exactly(2));
            this.accountRepository.Verify(ar => ar.Get(
                It.Is<Expression<Func<Account, bool>>>(
                    f => f.Compile().Invoke(correctAccount) && !f.Compile().Invoke(incorrectAccount))), Times.Exactly(2));

            messageAttachment.Verify(e => e.Download(Path.Combine(RootPath, subject, "20120201", "Menssage")), Times.Once());

            this.deliverableRepository.Verify(dr => dr.Insert(It.IsAny<Deliverable>()), Times.Once());
            this.attachmentRepository.Verify(ar => ar.Insert(It.IsAny<Attachment>()), Times.Once());

            this.deliverableRepository.Verify(dr => dr.Save(), Times.Once());
            this.groupRepository.Verify(gr => gr.Save(), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Once());
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
            Course previousCourse = new Course(2, 2011, 7510) {Id = 1};
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

            Account correctAccount = new Account { User = "ayudantes-7510@gmail.com" };
            correctAccount.Course = actualCourse;
            Account incorrectAccount = new Account { User = "teorica-7511@gmail.com" };
            this.accountRepository.Setup(
                ar =>
                ar.Get(
                    It.Is<Expression<Func<Account, bool>>>(
                        f => f.Compile().Invoke(correctAccount) && !f.Compile().Invoke(incorrectAccount)))).
                Returns(new List<Account> { correctAccount }).Verifiable();

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
            message.Setup(e => e.To).Returns(new List<string>() {DestinationAddress});
            message.Setup(e => e.Date).Returns(receptionDate);

            AddDeliverableToGroupDatabaseEntryAction action = CreateAction();

            // act
            action.Execute(message.Object);

            // validate
            this.studentRepository.Verify(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         f.Compile().Invoke(falseStudent))),
                Times.Once());
            this.accountRepository.Verify(ar => ar.Get(
                It.Is<Expression<Func<Account, bool>>>(
                    f => f.Compile().Invoke(correctAccount) && !f.Compile().Invoke(incorrectAccount))), Times.Exactly(2));

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


            Account incorrectAccount = new Account { User = "teorica-7511@gmail.com" };
            this.accountRepository.Setup(
                ar =>
                ar.Get(
                    It.Is<Expression<Func<Account, bool>>>(
                        f => !f.Compile().Invoke(incorrectAccount)))).
                Returns(new List<Account>()).Verifiable();

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
            this.accountRepository.Verify(ar => ar.Get(
                It.Is<Expression<Func<Account, bool>>>(
                    f => !f.Compile().Invoke(incorrectAccount))), Times.Exactly(2));
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotAddDeliverableSendedToInexistentCourse()
        {
            // arrange
            Student trueStudent = new Student(91363, "Matias", "message@address.com");
            Student falseStudent = new Student(90202, "Sebastian", "other.test@address.com");

            Course previousCourse = new Course(2, 2011, 7510) { Id = 1 };
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

            Account correctAccount = new Account { User = "ayudantes-7510@gmail.com" };
            correctAccount.Course = previousCourse;
            Account incorrectAccount = new Account { User = "teorica-7511@gmail.com" };
            this.accountRepository.Setup(
                ar =>
                ar.Get(
                    It.Is<Expression<Func<Account, bool>>>(
                        f => f.Compile().Invoke(correctAccount) && !f.Compile().Invoke(incorrectAccount)))).
                Returns(new List<Account> { correctAccount }).Verifiable();

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
            message.Setup(e => e.To).Returns(new List<string>{DestinationAddress});
            message.Setup(e => e.Date).Returns(receptionDate);

            AddDeliverableToGroupDatabaseEntryAction action = CreateAction();

            // act
            action.Execute(message.Object);

            // validate
            this.studentRepository.Verify(
                sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(trueStudent) &&
                                                                         f.Compile().Invoke(falseStudent))),
                Times.Once());
            this.accountRepository.Verify(ar => ar.Get(
                It.Is<Expression<Func<Account, bool>>>(
                    f => f.Compile().Invoke(correctAccount) && !f.Compile().Invoke(incorrectAccount))), Times.Exactly(2));

            this.courseRepository.Verify(cr => cr.Get(
                It.Is<Expression<Func<Course, bool>>>(
                    f => !f.Compile().Invoke(previousCourse))), Times.Exactly(1));
        }

        private AddDeliverableToGroupDatabaseEntryAction CreateAction()
        {
            return new AddDeliverableToGroupDatabaseEntryAction(this.courseManagementRepositories.Object,
                                                                this.configurationService.Object);
        }
    }
}