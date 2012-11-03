using System.Collections.Generic;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.MessageProcessing.Rules;
using CourseManagement.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    [TestClass]
    public class NewStudentInCourseRuleFixture
    {

        private MockRepository mockRepository;
        private Mock<IActionFactory> actionFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
        }

        [TestMethod]
        public void ShouldMatchMessage()
        {
            // arrange
            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ALTA-MATERIA-7510]91363-Matias Servetto");
            message.Setup(m => m.From).Returns("servetto.matias@gmail.com");

            NewStudentInCourseRule rule = CreateRule();

            Mock<IAction> action = mockRepository.Create<IAction>();
            action.Setup(a => a.Execute(message.Object)).Verifiable();
            this.actionFactory.Setup(af => af.CreateActions(rule.Name)).Returns(new List<IAction> {action.Object});

            // act
            rule.RetrieveActions();
            if (rule.IsMatch(message.Object))
                rule.Process(message.Object);

            // validate
            this.actionFactory.Verify(af => af.CreateActions(rule.Name), Times.Once());
            action.Verify(a => a.Execute(message.Object), Times.Once());
        }

        [TestMethod]
        public void ShouldNotMatchMessageWithWrongSubject()
        {
            // arrange
            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ALTA-MATERIA-]91363-Matias Servetto");
            message.Setup(m => m.From).Returns("servetto.matias@gmail.com");

            NewStudentInCourseRule rule = CreateRule();

            Mock<IAction> action = mockRepository.Create<IAction>();
            action.Setup(a => a.Execute(message.Object)).Verifiable();
            this.actionFactory.Setup(af => af.CreateActions(rule.Name)).Returns(new List<IAction> { action.Object });

            // act
            rule.RetrieveActions();
            if( rule.IsMatch(message.Object))
                rule.Process(message.Object);

            // validate
            this.actionFactory.Verify(af => af.CreateActions(rule.Name), Times.Once());
            action.Verify(a => a.Execute(message.Object), Times.Never());
        }

        private NewStudentInCourseRule CreateRule()
        {
            return new NewStudentInCourseRule(this.actionFactory.Object);
        }
    }
}
