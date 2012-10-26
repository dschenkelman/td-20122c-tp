using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.MessageProcessing.Tests.Actions
{
    [TestClass]
    public class AddDerivableToGroupDatabaseEntryActionFixture
    {
        private MockRepository mockRepository;
        private Mock<IRepository<Student>> studentRepository;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<IRepository<Group>> groupRepository;
        private Mock<IRepository<Deliverable>> deliverableRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.groupRepository = this.mockRepository.Create<IRepository<Group>>();
            this.deliverableRepository = this.mockRepository.Create<IRepository<Deliverable>>();
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();

            this.courseManagementRepositories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Groups).Returns(this.groupRepository.Object);
            this.courseManagementRepositories.Setup(cmr => cmr.Deliverables).Returns(this.deliverableRepository.Object);
        }

        [TestMethod]
        public void ShouldAddDeliverableToGroup()
        {
            // arrange
            Group correctGroup = new Group();
            correctGroup.Id = 1;
            Student student1CorrectGroup = new Student(91363, "Matias Servetto", "matias.servetto@gmail.com");
            Student student2CorrectGroup = new Student(90202, "Sebastian Rodriguez", "sebastianr213@gmail.com");
            correctGroup.Students.Add(student1CorrectGroup);
            correctGroup.Students.Add(student2CorrectGroup);
            Course course = new Course(1,2012,7510);
            correctGroup.Course = course;
            List<Student> students = new List<Student>{ student1CorrectGroup };

 /*           Group incorrectGroup = new Group();
            incorrectGroup.Id = 1;
            Student student1IncorrectGroup = new Student(91111, "Damian Schenkelman", "damian.schenkleman@gmail.com");
            Student student2IncorrectGroup = new Student(90333, "Pepe Lopez", "pepe@hotmail.com");
            incorrectGroup.Students.Add(student1IncorrectGroup);
            incorrectGroup.Students.Add(student2IncorrectGroup);*/

            this.studentRepository.Setup(sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(student1CorrectGroup)))).Returns( students ).Verifiable();
            this.studentRepository.Setup(sr => sr.Save()).Verifiable();


            const string DestinationAddress = "ayudantes-7510@gmail.com";
            string sourceAddress = student1CorrectGroup.MessagingSystemId;
            const int NumeroTp = 1;
            string Subject = "[ENTREGA-TP-" + NumeroTp + "]";
            DateTime receptionDate = new DateTime(2012, 5, 6);
            Mock<IMessage> message = mockRepository.Create<IMessage>();
            message.Setup(e => e.Subject).Returns(Subject);
            message.Setup(e => e.Address).Returns(sourceAddress);
            message.Setup(e => e.DestinationAddress).Returns(DestinationAddress);
            message.Setup(e => e.Date).Returns(receptionDate);

            AddDerivableToGroupDatabaseEntryAction action = CreateAction();

            // TODO implement

            // act
            action.Execute(message.Object);
            
            // assert
            Assert.AreEqual(1, correctGroup.Deliverables.Count());
            Assert.AreEqual(receptionDate, correctGroup.Deliverables.ElementAt(0).ReceptionDate);

            this.studentRepository.Verify(sr => sr.Get(It.Is<Expression<Func<Student, bool>>>(f => f.Compile().Invoke(student1CorrectGroup))), Times.Once());
            this.studentRepository.Verify(sr => sr.Save(), Times.Once());
        }

        private AddDerivableToGroupDatabaseEntryAction CreateAction()
        {
            return new AddDerivableToGroupDatabaseEntryAction(this.courseManagementRepositories.Object);
        }
    }
}
