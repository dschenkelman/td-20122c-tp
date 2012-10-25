using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CourseManagement.EmailProcessing.Actions;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;
using Microsoft.Moles.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CourseManagement.EmailProcessing.Tests.Actions
{
    [TestClass]
    public class AddGroupToCourseDatabaseEntryActionFixture
    {
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepostiories;
        private Mock<IRepository<Group>> groupRepository;
        private Mock<IEmail> email;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.courseManagementRepostiories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.groupRepository = this.mockRepository.Create<IRepository<Group>>();

            this.courseManagementRepostiories.Setup(cmr => cmr.Groups).Returns(this.groupRepository.Object);
            
            this.email = this.mockRepository.Create<IEmail>();
        }

        [TestMethod]
        public void ShouldAddNewGroupWhenUsingExecute()
        {
            // arrange
            const int wrongId = 2;

            var trueGroup = new Group();
            var falseGroup = new Group { Id = wrongId };
            var groups = new List<Group> ();

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroup)))))
                                        .Returns(groups)
                                        .Verifiable();

            this.groupRepository.Setup(gr => gr.Insert(It.Is<Group>(g => g.Id == trueGroup.Id))).Verifiable();

            this.groupRepository.Setup(gr => gr.Save()).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();

            // act

            action.Execute(email.Object);

            // assert

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroup)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert(It.Is<Group>(g => g.Id == trueGroup.Id)), Times.Once());

            this.groupRepository.Verify(gr => gr.Save(), Times.Once());

        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldNotAddAlreadyExistingGroupToCourseDatabaseWhenUsingExecute()
        {
            // arrange

            const int wrongId = 2;
            
            var trueGroup = new Group();
            var falseGroup = new Group { Id = wrongId };
            var groups = new List<Group> { trueGroup };

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroup)))))
                                        .Returns(groups)
                                        .Verifiable();

            this.groupRepository.Setup(gr => gr.Insert(It.Is<Group>(g => g.Id == trueGroup.Id))).Verifiable();
            
            this.groupRepository.Setup(gr => gr.Save()).Verifiable();
  
            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();
            
            // act
            action.Execute(email.Object);

            // assert

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroup)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert(It.Is<Group>(g => g.Id == trueGroup.Id)), Times.Never());

            this.groupRepository.Verify(gr => gr.Save(), Times.Never());

        }

        private AddGroupToCourseDatabaseEntryAction CreateAddGroupToCourseDatabaseEntryAction()
        {
            return new AddGroupToCourseDatabaseEntryAction(this.courseManagementRepostiories.Object);
        }
    }
}
