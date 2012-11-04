using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.MessageProcessing.Tests.Actions
{
    [TestClass]
    public class AddTicketToDatabaseActionFixture
    {
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> courseManagmentRepostiories;
        private Mock<IRepository<Ticket>> ticketRepository;
        private Mock<IRepository<Attachment>> attachmentRepository;
        private Mock<IRepository<Student>> studentRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.courseManagmentRepostiories = this.mockRepository.Create<ICourseManagementRepositories>();

            this.ticketRepository = this.mockRepository.Create<IRepository<Ticket>>();
            this.courseManagmentRepostiories.Setup(cmr => cmr.Tickets).Returns(this.ticketRepository.Object);

            this.attachmentRepository = this.mockRepository.Create<IRepository<Attachment>>();
            this.courseManagmentRepostiories.Setup(cmr => cmr.Attachments).Returns(this.attachmentRepository.Object);

            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseManagmentRepostiories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
        }

        [TestMethod]
        public void ShouldAddTicketToDatabase()
        {
            // arrange
            this.attachmentRepository.Setup(tr => tr.Save()).Verifiable();

            const string MessageSystemId = "matias.servetto@gmail.com";
            Student trueStudent = new Student(91363, "Matias Servetto", MessageSystemId),
                    falseStudent = new Student(91363, "Damian Schenkelman", "dami.schklmn@gmail.com");

            this.studentRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).Returns(
                            new List<Student>{ trueStudent }).
                Verifiable();

            const string MessageSubject = "[CONSULTA-PUBLICA]Topic";
            Ticket trueTicket = new Ticket {MessageSubject = MessageSubject},
                   falseTicket = new Ticket {MessageSubject = "[CONSULTA-PUBLICA]OtherTopic"};

            this.ticketRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Ticket, bool>>>(
                        f => f.Compile().Invoke(trueTicket) && !f.Compile().Invoke(falseTicket)))).Returns(
                            new List<Ticket>()).
                Verifiable();
            this.ticketRepository.Setup(
                tr => tr.Insert(It.IsAny<Ticket>())).Verifiable();
            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.From).Returns(MessageSystemId).Verifiable();
            message.Setup(m => m.Subject).Returns(MessageSubject).Verifiable();
            message.Setup(m => m.Body).Returns("body message").Verifiable();
            message.Setup(m => m.Date).Returns(new DateTime(2012,2,2)).Verifiable();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment>()).Verifiable();

            AddTicketToDatabaseAction action = CreateAction();

            //act
            action.Execute(message.Object);

            //validate
            this.studentRepository.Verify(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))), Times.Once());

            this.ticketRepository.Verify(
                tr => tr.Insert(It.IsAny<Ticket>()), Times.Once());
            this.ticketRepository.Verify(tr => tr.Save(), Times.Once());
        }

        [TestMethod]
        public void ShouldAddTicketToDatabaseSavingAttachments()
        {
            // arrange
            const string MessageSystemId = "matias.servetto@gmail.com";
            Student trueStudent = new Student(91363, "Matias Servetto", MessageSystemId),
                    falseStudent = new Student(91363, "Damian Schenkelman", "dami.schklmn@gmail.com");

            this.studentRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).Returns(
                            new List<Student> { trueStudent }).
                Verifiable();

            Mock<IMessageAttachment> attachment1 = this.mockRepository.Create<IMessageAttachment>(),
                                     attachment2 = this.mockRepository.Create<IMessageAttachment>(),
                                     attachment3 = this.mockRepository.Create<IMessageAttachment>();
            attachment1.Setup(a => a.Name).Returns("Attachment1");
            attachment2.Setup(a => a.Name).Returns("Attachment2");
            attachment3.Setup(a => a.Name).Returns("Attachment3");
            attachment1.Setup(a => a.Download(It.IsAny<String>()));
            attachment2.Setup(a => a.Download(It.IsAny<String>()));
            attachment3.Setup(a => a.Download(It.IsAny<String>()));

            this.attachmentRepository.Setup(
                tr => tr.Insert(It.IsAny<Attachment>())).Verifiable();
            this.attachmentRepository.Setup(tr => tr.Save()).Verifiable();

            const string MessageSubject = "[CONSULTA-PUBLICA]Topic";
            Ticket trueTicket = new Ticket { MessageSubject = MessageSubject },
                   falseTicket = new Ticket { MessageSubject = "[CONSULTA-PUBLICA]OtherTopic" };

            this.ticketRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Ticket, bool>>>(
                        f => f.Compile().Invoke(trueTicket) && !f.Compile().Invoke(falseTicket)))).Returns(
                            new List<Ticket>()).
                Verifiable();
            this.ticketRepository.Setup(
                tr => tr.Insert(It.IsAny<Ticket>())).Verifiable();
            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.From).Returns(MessageSystemId).Verifiable();
            message.Setup(m => m.Subject).Returns(MessageSubject).Verifiable();
            message.Setup(m => m.Body).Returns("body message").Verifiable();
            message.Setup(m => m.Date).Returns(new DateTime(2012, 2, 2)).Verifiable();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment>
                                                          {attachment1.Object, attachment2.Object, attachment3.Object}).
                Verifiable();

            AddTicketToDatabaseAction action = CreateAction();

            //act
            action.Execute(message.Object);

            //validate
            this.studentRepository.Verify(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))), Times.Once());

            this.attachmentRepository.Verify(ar => ar.Insert(It.IsAny<Attachment>()), Times.Exactly(3));
            this.attachmentRepository.Verify(ar => ar.Save(), Times.Once());

            this.ticketRepository.Verify(
                tr => tr.Insert(It.IsAny<Ticket>()), Times.Once());
            this.ticketRepository.Verify(tr => tr.Save(), Times.Once());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotAddExistingTicketToDatabaseNorSaveAttachments()
        {
            // arrange
            const string MessageSystemId = "matias.servetto@gmail.com";
            Student trueStudent = new Student(91363, "Matias Servetto", MessageSystemId),
                    falseStudent = new Student(91363, "Damian Schenkelman", "dami.schklmn@gmail.com");

            this.studentRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).Returns(
                            new List<Student> { trueStudent }).
                Verifiable();

            Mock<IMessageAttachment> attachment1 = this.mockRepository.Create<IMessageAttachment>(),
                                     attachment2 = this.mockRepository.Create<IMessageAttachment>(),
                                     attachment3 = this.mockRepository.Create<IMessageAttachment>();
            attachment1.Setup(a => a.Name).Returns("Attachment1");
            attachment2.Setup(a => a.Name).Returns("Attachment2");
            attachment3.Setup(a => a.Name).Returns("Attachment3");
            attachment1.Setup(a => a.Download(It.IsAny<String>()));
            attachment2.Setup(a => a.Download(It.IsAny<String>()));
            attachment3.Setup(a => a.Download(It.IsAny<String>()));

            const string MessageSubject = "[CONSULTA-PUBLICA]Topic";
            Ticket trueTicket = new Ticket { MessageSubject = MessageSubject },
                   falseTicket = new Ticket { MessageSubject = "[CONSULTA-PUBLICA]OtherTopic" };

            this.ticketRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Ticket, bool>>>(
                        f => f.Compile().Invoke(trueTicket) && !f.Compile().Invoke(falseTicket)))).Returns(
                            new List<Ticket>{ trueTicket }).
                Verifiable();
            this.attachmentRepository.Setup(
                tr => tr.Insert(It.IsAny<Attachment>())).Verifiable();
            this.attachmentRepository.Setup(tr => tr.Save()).Verifiable();

            this.ticketRepository.Setup(
                tr => tr.Insert(It.IsAny<Ticket>())).Verifiable();
            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.From).Returns(MessageSystemId).Verifiable();
            message.Setup(m => m.Subject).Returns(MessageSubject).Verifiable();
            message.Setup(m => m.Body).Returns("body message").Verifiable();
            message.Setup(m => m.Date).Returns(new DateTime(2012, 2, 2)).Verifiable();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment> { attachment1.Object, attachment2.Object, attachment3.Object }).
                Verifiable();

            AddTicketToDatabaseAction action = CreateAction();

            //act
            action.Execute(message.Object);

            //validate
            this.studentRepository.Verify(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))), Times.Once());

            this.attachmentRepository.Verify(ar => ar.Insert(It.IsAny<Attachment>()), Times.Never());
            this.attachmentRepository.Verify(ar => ar.Save(), Times.Never());

            this.ticketRepository.Verify(
                tr => tr.Insert(It.IsAny<Ticket>()), Times.Never());
            this.ticketRepository.Verify(tr => tr.Save(), Times.Never());
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ShouldNotAddTicketToDatabaseNorSaveAttachmentsFromAnNonRegisteredMessageSystemId()
        {
            // arrange
            const string MessageSystemId = "matias.servetto@gmail.com";
            Student trueStudent = new Student(91363, "Matias Servetto", MessageSystemId),
                    falseStudent = new Student(91363, "Damian Schenkelman", "dami.schklmn@gmail.com");

            this.studentRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).Returns(
                            new List<Student>()).
                Verifiable();

            Mock<IMessageAttachment> attachment1 = this.mockRepository.Create<IMessageAttachment>(),
                                     attachment2 = this.mockRepository.Create<IMessageAttachment>(),
                                     attachment3 = this.mockRepository.Create<IMessageAttachment>();
            attachment1.Setup(a => a.Name).Returns("Attachment1");
            attachment2.Setup(a => a.Name).Returns("Attachment2");
            attachment3.Setup(a => a.Name).Returns("Attachment3");
            attachment1.Setup(a => a.Download(It.IsAny<String>()));
            attachment2.Setup(a => a.Download(It.IsAny<String>()));
            attachment3.Setup(a => a.Download(It.IsAny<String>()));

            const string MessageSubject = "[CONSULTA-PUBLICA]Topic";
            Ticket trueTicket = new Ticket { MessageSubject = MessageSubject },
                   falseTicket = new Ticket { MessageSubject = "[CONSULTA-PUBLICA]OtherTopic" };

            this.ticketRepository.Setup(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Ticket, bool>>>(
                        f => f.Compile().Invoke(trueTicket) && !f.Compile().Invoke(falseTicket)))).Returns(
                            new List<Ticket> { trueTicket }).
                Verifiable();
            this.attachmentRepository.Setup(
                tr => tr.Insert(It.IsAny<Attachment>())).Verifiable();
            this.attachmentRepository.Setup(tr => tr.Save()).Verifiable();

            this.ticketRepository.Setup(
                tr => tr.Insert(It.IsAny<Ticket>())).Verifiable();
            this.ticketRepository.Setup(tr => tr.Save()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.From).Returns(MessageSystemId).Verifiable();
            message.Setup(m => m.Subject).Returns(MessageSubject).Verifiable();
            message.Setup(m => m.Body).Returns("body message").Verifiable();
            message.Setup(m => m.Date).Returns(new DateTime(2012, 2, 2)).Verifiable();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment> { attachment1.Object, attachment2.Object, attachment3.Object }).
                Verifiable();

            AddTicketToDatabaseAction action = CreateAction();

            //act
            action.Execute(message.Object);

            //validate
            this.studentRepository.Verify(
                tr =>
                tr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))), Times.Once());

            this.attachmentRepository.Verify(ar => ar.Insert(It.IsAny<Attachment>()), Times.Never());
            this.attachmentRepository.Verify(ar => ar.Save(), Times.Never());

            this.ticketRepository.Verify(
                tr => tr.Insert(It.IsAny<Ticket>()), Times.Never());
            this.ticketRepository.Verify(tr => tr.Save(), Times.Never());
        }

        private AddTicketToDatabaseAction CreateAction()
        {
            return new AddTicketToDatabaseAction(this.courseManagmentRepostiories.Object);
        }
    }
}
