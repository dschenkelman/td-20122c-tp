using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.MessageProcessing.Rules;
using CourseManagement.Messages;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    [TestClass]
    public class AddDeliverableToGroupDatabaseEntryRuleFixture
    {
        private MockRepository mockRepository;
        private Mock<IActionFactory> actionFactory;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<IRepository<Student>> studentRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();

            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();

            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);

        }

        [TestMethod]
        public void ShouldMatchMessage()
        {
            // arrange
            const string TrueMessageSystemId = "servetto.matias@gmail.com",
                      FalseMessageSystemId = "damian.schenkelman@gmail.com";

            Student trueStudent = new Student(91363, "Matias Servetto", TrueMessageSystemId),
                    falseStudent = new Student(91363, "Matias Servetto", FalseMessageSystemId);

            this.studentRepository.Setup(
                sr =>
                sr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).
                Returns(new List<Student> {trueStudent}).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ENTREGA-TP-1]");
            message.Setup(m => m.From).Returns(TrueMessageSystemId);
            Mock<IMessageAttachment> messageAttachment = this.mockRepository.Create<IMessageAttachment>();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment> {messageAttachment.Object}).Verifiable();

            AddDeliverableToGroupDatabaseEntryRule rule = CreateRule();
            rule.Initialize(new RuleEntry("AddDeliverable", "^\\[ENTREGA-TP-[0-9]+\\]$"));

            // act and validate
            Assert.IsTrue(rule.IsMatch(message.Object, false));

            message.Verify(m => m.Attachments, Times.Once());
            this.studentRepository.Verify(sr =>
                                          sr.Get(
                                              It.Is<Expression<Func<Student, bool>>>(
                                                  f =>
                                                  f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))),
                                          Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithWrongSubject()
        {
            // arrange
            const string TrueMessageSystemId = "servetto.matias@gmail.com",
                         FalseMessageSystemId = "damian.schenkelman@gmail.com";

            Student trueStudent = new Student(91363, "Matias Servetto", TrueMessageSystemId),
                    falseStudent = new Student(91363, "Matias Servetto", FalseMessageSystemId);

            this.studentRepository.Setup(
                sr =>
                sr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).
                Returns(new List<Student> {trueStudent}).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[TP-5]");
            message.Setup(m => m.From).Returns(TrueMessageSystemId);
            Mock<IMessageAttachment> messageAttachment = this.mockRepository.Create<IMessageAttachment>();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment> { messageAttachment.Object }).Verifiable();

            AddDeliverableToGroupDatabaseEntryRule rule = CreateRule();
            rule.Initialize(new RuleEntry("AddDeliverable", "^\\[ENTREGA-TP-[0-9]+\\]$"));

            // act and validate
            Assert.IsFalse(rule.IsMatch(message.Object, false));

            message.Verify(m => m.Attachments, Times.Never());
            this.studentRepository.Verify(sr =>
                              sr.Get(
                                  It.Is<Expression<Func<Student, bool>>>(
                                      f =>
                                      f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))),
                              Times.Never());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithoutAttachments()
        {
            // arrange
            const string TrueMessageSystemId = "servetto.matias@gmail.com",
                         FalseMessageSystemId = "damian.schenkelman@gmail.com";

            Student trueStudent = new Student(91363, "Matias Servetto", TrueMessageSystemId),
                    falseStudent = new Student(91363, "Matias Servetto", FalseMessageSystemId);

            this.studentRepository.Setup(
                sr =>
                sr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).
                Returns(new List<Student> { trueStudent }).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ENTREGA-TP-5]");
            message.Setup(m => m.From).Returns(TrueMessageSystemId);
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment>()).Verifiable();

            AddDeliverableToGroupDatabaseEntryRule rule = CreateRule();
            rule.Initialize(new RuleEntry("AddDeliverable", "^\\[ENTREGA-TP-[0-9]+\\]$"));

            // act and validate
            Assert.IsFalse(rule.IsMatch(message.Object, false));
            
            message.Verify(m => m.Attachments, Times.Once());
            this.studentRepository.Verify(sr =>
                              sr.Get(
                                  It.Is<Expression<Func<Student, bool>>>(
                                      f =>
                                      f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))),
                              Times.Never());
        }

        [TestMethod]
        public void ShouldNotMatchMessageIfTheMessageSystemIdDoesNotExists()
        {
            // arrange
            const string TrueMessageSystemId = "servetto.matias@gmail.com",
                         FalseMessageSystemId = "damian.schenkelman@gmail.com";

            Student trueStudent = new Student(91363, "Matias Servetto", TrueMessageSystemId),
                    falseStudent = new Student(91363, "Matias Servetto", FalseMessageSystemId);

            this.studentRepository.Setup(
                sr =>
                sr.Get(
                    It.Is<Expression<Func<Student, bool>>>(
                        f => f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent)))).
                Returns(new List<Student>()).Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ENTREGA-TP-5]");
            message.Setup(m => m.From).Returns(TrueMessageSystemId);
            Mock<IMessageAttachment> messageAttachment = this.mockRepository.Create<IMessageAttachment>();
            message.Setup(m => m.Attachments).Returns(new List<IMessageAttachment> { messageAttachment.Object }).Verifiable();

            AddDeliverableToGroupDatabaseEntryRule rule = CreateRule();
            rule.Initialize(new RuleEntry("AddDeliverable", "^\\[ENTREGA-TP-[0-9]+\\]$"));

            // act and validate
            Assert.IsFalse(rule.IsMatch(message.Object, false));

            message.Verify(m => m.Attachments, Times.Once());
            this.studentRepository.Verify(sr =>
                              sr.Get(
                                  It.Is<Expression<Func<Student, bool>>>(
                                      f =>
                                      f.Compile().Invoke(trueStudent) && !f.Compile().Invoke(falseStudent))),
                              Times.Once());
        }

        private AddDeliverableToGroupDatabaseEntryRule CreateRule()
        {
            return new AddDeliverableToGroupDatabaseEntryRule(this.courseManagementRepositories.Object, this.actionFactory.Object);
        }
    }
}
