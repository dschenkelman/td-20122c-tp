using CourseManagement.MessageProcessing.Actions;

namespace CourseManagement.MessageProcessing.Tests.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class AddGroupToCourseDatabaseEntryActionFixture
    {
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> courseManagementRepostiories;
        private Mock<IRepository<Group>> groupRepository;
        private Mock<IRepository<Account>> accountRepository;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<IMessage> message;
        
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.courseManagementRepostiories = this.mockRepository.Create<ICourseManagementRepositories>();
            
            this.accountRepository = this.mockRepository.Create<IRepository<Account>>();
            this.courseManagementRepostiories.Setup(cmr => cmr.Accounts).Returns(this.accountRepository.Object);

            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.courseManagementRepostiories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);

            this.groupRepository = this.mockRepository.Create<IRepository<Group>>();
            this.courseManagementRepostiories.Setup(cmr => cmr.Groups).Returns(this.groupRepository.Object);
            
            this.message = this.mockRepository.Create<IMessage>();
        }

        [TestMethod]
        public void ShouldAddNewGroupWhenUsingExecute()
        {
            // arrange
            const int Year = 2012;
            const int WrongYear = 2000;
            const int CorrectMonth = 10;
            const int Day = 25;
            const int Semester = 2;
            const int WrongSemester = 1;
            const int SubjectId = 7510;
            const int WrongSubjectId = 7515;

            var correctDate = new DateTime(Year,CorrectMonth,Day);

            const string MessageAddress = "sebas@gmail.com";

            var account = new Account {User = MessageAddress,CourseCode = SubjectId};
            var accounts = new List<Account> {account};

            var trueCourse = new Course(Semester,Year,SubjectId);
            var falseCourseWrongYear = new Course(Semester, WrongYear, SubjectId);
            var falseCourseWrongSemester = new Course(WrongSemester, Year, SubjectId);
            var falseCourseWrongSubjectId = new Course(Semester, Year, WrongSubjectId);
            var falseCourse = new Course(WrongSemester, WrongYear, WrongSubjectId);

            var courses = new List<Course> {trueCourse};

            // Hardcoded group's id's
            var trueGroup = new Group { Course = trueCourse };
            var falseGroupWrongYear = new Group { Course = falseCourseWrongYear, Id = 1 };
            var falseGroupWrongSemester = new Group { Course = falseCourseWrongSemester, Id = 2 };
            var falseGroupWrongSubjectId = new Group { Course = falseCourseWrongSubjectId, Id = 3 };
            var falseGroupWrong = new Group { Course = falseCourse, Id = 4 };

            var groups = new List<Group>();

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>(f =>
                                                         f.Compile().Invoke(trueGroup) &&
                                                         (!f.Compile().Invoke(falseGroupWrongYear))
                                                         && (!f.Compile().Invoke(falseGroupWrongSemester)) &&
                                                         (!f.Compile().Invoke(falseGroupWrongSubjectId))
                                                         && (!f.Compile().Invoke(falseGroupWrong)))))
                                        .Returns(groups)
                                        .Verifiable();

            this.groupRepository.Setup(gr => gr.Insert(It.Is<Group>(g => (g.Id == trueGroup.Id) && (g.Course == trueGroup.Course)))).Verifiable();

            this.groupRepository.Setup(gr => gr.Save()).Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account)))).Returns(accounts).Verifiable();

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f => 
                                                          f.Compile().Invoke(trueCourse) &&
                                                          (!f.Compile().Invoke(falseCourseWrongYear))
                                                          && (!f.Compile().Invoke(falseCourseWrongSemester)) &&
                                                          (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                                          && (!f.Compile().Invoke(falseCourse))))).Returns(courses).Verifiable();

            this.message.Setup(e => e.Address).Returns(MessageAddress).Verifiable();

            this.message.Setup(e => e.Date).Returns(correctDate).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();
            

            // act

            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.Address , Times.Once());

            this.message.Verify(e => e.Date , Times.Exactly(2));

            this.accountRepository.Verify(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => (f.Compile().Invoke(account)))), Times.Once());

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                                        (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear))
                                        && (!f.Compile().Invoke(falseCourseWrongSemester)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                        && (!f.Compile().Invoke(falseCourse)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongSubjectId))
                                        && (!f.Compile().Invoke(falseGroupWrong)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert(It.Is<Group>(g => (g.Id == trueGroup.Id) && (g.Course == trueGroup.Course))), Times.Once());

            this.groupRepository.Verify(gr => gr.Save(), Times.Once());

        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldNotAddAlreadyExistingGroupToCourseDatabaseWhenUsingExecute()
        {
            // arrange
            const int Year = 2012;
            const int WrongYear = 2000;
            const int CorrectMonth = 10;
            const int Day = 25;
            const int Semester = 2;
            const int WrongSemester = 1;
            const int SubjectId = 7510;
            const int WrongSubjectId = 7515;

            var correctDate = new DateTime(Year, CorrectMonth, Day);

            const string MessageAddress = "sebas@gmail.com";

            var account = new Account { User = MessageAddress, CourseCode = SubjectId };
            var accounts = new List<Account> { account };

            var trueCourse = new Course(Semester, Year, SubjectId);
            var falseCourseWrongYear = new Course(Semester, WrongYear, SubjectId);
            var falseCourseWrongSemester = new Course(WrongSemester, Year, SubjectId);
            var falseCourseWrongSubjectId = new Course(Semester, Year, WrongSubjectId);
            var falseCourse = new Course(WrongSemester, WrongYear, WrongSubjectId);

            var courses = new List<Course> { trueCourse };

            //Hardcoded group's id's
            var trueGroup = new Group { Course = trueCourse };
            var falseGroupWrongYear = new Group { Course = falseCourseWrongYear, Id = 1 };
            var falseGroupWrongSemester = new Group { Course = falseCourseWrongSemester, Id = 2 };
            var falseGroupWrongSubjectId = new Group { Course = falseCourseWrongSubjectId, Id = 3 };
            var falseGroupWrong = new Group { Course = falseCourse, Id = 4 };

            var groups = new List<Group> { trueGroup };

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongSubjectId))
                                        && (!f.Compile().Invoke(falseGroupWrong)))))
                                        .Returns(groups)
                                        .Verifiable();

            this.groupRepository.Setup(gr => gr.Insert(It.Is<Group>(g => (g.Id == trueGroup.Id) && (g.Course == trueGroup.Course))))
                                .Verifiable();

            this.groupRepository.Setup(gr => gr.Save())
                                .Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => (f.Compile().Invoke(account)))))
                                    .Returns(accounts)
                                    .Verifiable();

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                                        (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear))
                                        && (!f.Compile().Invoke(falseCourseWrongSemester)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                        && (!f.Compile().Invoke(falseCourse)))))
                                        .Returns(courses)
                                        .Verifiable();

            this.message.Setup(e => e.Address)
                        .Returns(MessageAddress)
                        .Verifiable();

            this.message.Setup(e => e.Date)
                        .Returns(correctDate)
                        .Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();


            // act

            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.Address, Times.Once());

            this.message.Verify(e => e.Date, Times.Exactly(2));

            this.accountRepository.Verify(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account))), Times.Once());

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                                                                            f.Compile().Invoke(trueCourse) &&
                                                                                            (!f.Compile().Invoke(falseCourseWrongYear)) &&
                                                                                            (!f.Compile().Invoke(falseCourseWrongSemester)) && (!f.Compile().Invoke(falseCourseWrongSubjectId)) && 
                                                                                            (!f.Compile().Invoke(falseCourse)))),
                                                                                            Times.Once());

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongSubjectId))
                                        && (!f.Compile().Invoke(falseGroupWrong)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert(It.Is<Group>(g => (g.Id == trueGroup.Id) && (g.Course == trueGroup.Course))), Times.Never());

            this.groupRepository.Verify(gr => gr.Save(), Times.Never());
        }

        private AddGroupToCourseDatabaseEntryAction CreateAddGroupToCourseDatabaseEntryAction()
        {
            return new AddGroupToCourseDatabaseEntryAction(this.courseManagementRepostiories.Object);
        }
    }
}
