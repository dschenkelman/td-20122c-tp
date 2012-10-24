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
            const int correctYear = 2012;
            const int incorrectYear = 2000;

            const int correctSemester = 2;
            const int incorrectSemester = 1;

            var trueGroup = new Group(correctYear,correctSemester);

            var falseGroupWrongYearAndSemester = new Group(incorrectYear, incorrectSemester);

            var falseGroupWrongYear = new Group(incorrectYear, correctSemester);

            var falseGroupWrongSemester = new Group(correctYear, incorrectSemester);

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongYearAndSemester)))))
                                        .Returns(new List<Group>())
                                        .Verifiable();
            
            this.groupRepository.Setup(gr => gr.Insert(It.Is<Group>(g => g.Year == correctYear && g.Semester == correctSemester && g.Id == trueGroup.Id)));

            this.groupRepository.Setup(gr => gr.Save()).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();

            // act

            action.Execute(email.Object);

            // assert

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongYearAndSemester)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert(It.Is<Group>(g => g.Year == correctYear && g.Semester == correctSemester && g.Id == trueGroup.Id)), Times.Once());

            this.groupRepository.Verify(gr => gr.Save(), Times.Once());

        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldNotAddAlreadyExistingGroupToCourseDatabaseWhenUsingExecute()
        {
            // arrange
            const int year = 2012;
            const int semester = 2;
            var group = new Group(year, semester);

            var groups = new List<Group> {group};

            const int correctYear = 2012;
            const int incorrectYear = 2000;

            const int correctSemester = 2;
            const int incorrectSemester = 1;

            var trueGroup = new Group(correctYear, correctSemester);

            var falseGroupWrongYearAndSemester = new Group(incorrectYear, incorrectSemester);

            var falseGroupWrongYear = new Group(incorrectYear, correctSemester);

            var falseGroupWrongSemester = new Group(correctYear, incorrectSemester);

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongYearAndSemester)))))
                                        .Returns(groups)
                                        .Verifiable();

            this.groupRepository.Setup(gr => gr.Insert(It.Is<Group>(g => g.Year == correctYear && g.Semester == correctSemester && g.Id == trueGroup.Id)));
            
            this.groupRepository.Setup(gr => gr.Save()).Verifiable();
  
            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            Mock<IEmail> email = mockRepository.Create<IEmail>();
            
            // act
            action.Execute(email.Object);

            // assert

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongYearAndSemester)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert(It.Is<Group>(g => g.Year == correctYear && g.Semester == correctSemester && g.Id == trueGroup.Id)), Times.Never());

            this.groupRepository.Verify(gr => gr.Save(), Times.Never());

        }

        private AddGroupToCourseDatabaseEntryAction CreateAddGroupToCourseDatabaseEntryAction()
        {
            return new AddGroupToCourseDatabaseEntryAction(this.courseManagementRepostiories.Object);
        }
    }
}
