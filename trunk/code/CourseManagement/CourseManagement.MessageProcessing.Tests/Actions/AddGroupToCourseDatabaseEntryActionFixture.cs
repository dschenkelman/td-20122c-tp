using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;

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

        private int year;
        private int wrongYear;
        private int correctMonth;
        private int day;
        private int semester;
        private int wrongSemester;
        private int subjectId;
        private int wrongSubjectId;
        
        private DateTime correctDate;
        private DateTime incorrectDateWrongYear;
        
        private string messageAddress;

        private Course trueCourse;
        private Course falseCourseWrongYear;
        private Course falseCourseWrongSemester;
        private Course falseCourseWrongSubjectId;
        private Course falseCourse;


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

            // common const values
            this.year = 2012;
            this.wrongYear = 2000;
            this.correctMonth = 10;
            this.day = 25;
            this.semester = 2;
            this.wrongSemester = 1;
            this.subjectId = 7510;
            this.wrongSubjectId = 7515;

            this.correctDate = new DateTime(this.year, this.correctMonth, this.day);
            this.incorrectDateWrongYear = new DateTime(this.wrongYear, this.correctMonth, this.day);

            this.trueCourse = new Course(this.semester, this.year, this.subjectId);
            this.falseCourseWrongYear = new Course(this.semester, this.wrongYear, this.subjectId);
            this.falseCourseWrongSemester = new Course(this.wrongSemester, this.year, this.subjectId);
            this.falseCourseWrongSubjectId = new Course(this.semester, this.year, this.wrongSubjectId);
            this.falseCourse = new Course(this.wrongSemester, this.wrongYear, this.wrongSubjectId);

            this.messageAddress = "sebas@gmail.com";

        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldThrowNonExistingCourseExceptionWhenUsingExecute()
        {
            //arrange
            var courses = new List<Course> {trueCourse};

            var account = new Account { User = this.messageAddress, CourseCode = this.subjectId };
            var accounts = new List<Account> { account };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                                        f.Compile().Invoke(falseCourseWrongYear) && (! f.Compile().Invoke(trueCourse)))))
                                                        .Returns(default(IEnumerable<Course>))
                                                        .Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account)))).Returns(accounts).Verifiable();

            this.message.Setup(e => e.To).Returns(this.messageAddress).Verifiable();

            this.message.Setup(e => e.Date).Returns(incorrectDateWrongYear).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();
            
            // act
            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.To, Times.Once());

            this.message.Verify(e => e.Date, Times.Exactly(2));

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                            (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear)))), Times.Once());

        }
        /*
        [TestMethod]
        public void ShouldThrowNoExistingStudentInDatabaseExceptionWhenUsingExecute()
        {
            //arrange
            var courses = new List<Course> { trueCourse };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                            f.Compile().Invoke(trueCourse) && (!f.Compile().Invoke(falseCourse))
                                            && (!f.Compile().Invoke(falseCourseWrongYear)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                            && (!f.Compile().Invoke(falseCourseWrongSemester)))))
                                            .Returns(courses)
                                            .Verifiable();
        }

        [TestMethod]
        public void ShouldAddNewGroupWhenUsingExecute()
        {
            
            //arrange
            var account = new Account { User = this.messageAddress, CourseCode = this.subjectId };
            var accounts = new List<Account> {account};

            var trueCourse = new Course(this.semester, this.year, this.subjectId);
            var falseCourseWrongYear = new Course(this.semester, this.wrongYear, this.subjectId);
            var falseCourseWrongSemester = new Course(this.wrongSemester, this.year, this.subjectId);
            var falseCourseWrongSubjectId = new Course(this.semester, this.year, this.wrongSubjectId);
            var falseCourse = new Course(this.wrongSemester, this.wrongYear, this.wrongSubjectId);

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

            this.message.Setup(e => e.To).Returns(this.messageAddress).Verifiable();

            this.message.Setup(e => e.Date).Returns(correctDate).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();
            

            // act

            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.To , Times.Once());

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


        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldNotAddAlreadyExistingGroupToCourseDatabaseWhenUsingExecute()
        {
            
            //arrange
            var account = new Account { User = this.messageAddress, CourseCode = this.subjectId };
            var accounts = new List<Account> { account };



            var courses = new List<Course> { trueCourse };

            // Hardcoded group's id's
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

            this.message.Setup(e => e.From)
                        .Returns(this.messageAddress)
                        .Verifiable();

            this.message.Setup(e => e.Date)
                        .Returns(correctDate)
                        .Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();


            // act

            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.From, Times.Once());

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
        }*/

        private AddGroupToCourseDatabaseEntryAction CreateAddGroupToCourseDatabaseEntryAction()
        {
            return new AddGroupToCourseDatabaseEntryAction(this.courseManagementRepostiories.Object);
        }
    }
}
