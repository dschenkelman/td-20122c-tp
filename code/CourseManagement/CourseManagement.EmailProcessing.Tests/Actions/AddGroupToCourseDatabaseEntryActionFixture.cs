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
        private Mock<IRepository<Account>> accountRepository;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<IEmail> email;
        


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
            
            this.email = this.mockRepository.Create<IEmail>();
        }

        [TestMethod]
        public void ShouldAddNewGroupWhenUsingExecute()
        {
            // arrange

            const int year = 2012;
            const int wrongYear = 2000;
            const int correctMonth = 10;
            const int day = 25;
            const int semester = 2;
            const int wrongSemester = 1;
            const int subjectId = 7510;
            const int wrongSubjectId = 7515;

            var correctDate = new DateTime(year,correctMonth,day);

            const string emailAddress = "sebas@gmail.com";

            var account = new Account {User = emailAddress,SubjectCode = subjectId};
            var accounts = new List<Account> {account};

            var trueCourse = new Course(semester,year,subjectId);
            var falseCourseWrongYear = new Course(semester, wrongYear, subjectId);
            var falseCourseWrongSemester = new Course(wrongSemester, year, subjectId);
            var falseCourseWrongSubjectId = new Course(semester, year, wrongSubjectId);
            var falseCourse = new Course(wrongSemester, wrongYear, wrongSubjectId);

            var courses = new List<Course> {trueCourse};

            //Hardcoded group's id's
            var trueGroup = new Group {Course = trueCourse};
            var falseGroupWrongYear = new Group { Course = falseCourseWrongYear , Id = 1};
            var falseGroupWrongSemester = new Group { Course = falseCourseWrongSemester, Id = 2};
            var falseGroupWrongSubjectId = new Group { Course = falseCourseWrongSubjectId, Id = 3};
            var falseGroupWrong = new Group { Course = falseCourse, Id = 4};

            var groups = new List<Group> ();

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongSubjectId))
                                        && (!f.Compile().Invoke(falseGroupWrong)))))
                                        .Returns(groups)
                                        .Verifiable();

            this.groupRepository.Setup(gr => gr.Insert(It.Is<Group>(g => (g.Id == trueGroup.Id) && (g.Course == trueGroup.Course)))).Verifiable();

            this.groupRepository.Setup(gr => gr.Save()).Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => (f.Compile().Invoke(account))))).Returns(accounts).Verifiable();

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                                        (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear))
                                        && (!f.Compile().Invoke(falseCourseWrongSemester)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                        && (!f.Compile().Invoke(falseCourse))))).Returns(courses).Verifiable();

            this.email.Setup(e => e.Address).Returns(emailAddress).Verifiable();

            this.email.Setup(e => e.Date).Returns(correctDate).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();
            

            // act

            action.Execute(this.email.Object);

            // assert

            this.email.Verify(e => e.Address , Times.Once());

            this.email.Verify(e => e.Date , Times.Exactly(2));

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

            const int year = 2012;
            const int wrongYear = 2000;
            const int correctMonth = 10;
            const int day = 25;
            const int semester = 2;
            const int wrongSemester = 1;
            const int subjectId = 7510;
            const int wrongSubjectId = 7515;

            var correctDate = new DateTime(year, correctMonth, day);

            const string emailAddress = "sebas@gmail.com";

            var account = new Account { User = emailAddress, SubjectCode = subjectId };
            var accounts = new List<Account> { account };

            var trueCourse = new Course(semester, year, subjectId);
            var falseCourseWrongYear = new Course(semester, wrongYear, subjectId);
            var falseCourseWrongSemester = new Course(wrongSemester, year, subjectId);
            var falseCourseWrongSubjectId = new Course(semester, year, wrongSubjectId);
            var falseCourse = new Course(wrongSemester, wrongYear, wrongSubjectId);

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

            this.email.Setup(e => e.Address)
                        .Returns(emailAddress)
                        .Verifiable();

            this.email.Setup(e => e.Date)
                        .Returns(correctDate)
                        .Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();


            // act

            action.Execute(this.email.Object);

            // assert

            this.email.Verify(e => e.Address, Times.Once());

            this.email.Verify(e => e.Date, Times.Exactly(2));

            this.accountRepository.Verify(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => (f.Compile().Invoke(account)))), Times.Once());

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                                        (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear))
                                        && (!f.Compile().Invoke(falseCourseWrongSemester)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                        && (!f.Compile().Invoke(falseCourse)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>
                                        (f => (f.Compile().Invoke(trueGroup)) && (!f.Compile().Invoke(falseGroupWrongYear))
                                        && (!f.Compile().Invoke(falseGroupWrongSemester)) && (!f.Compile().Invoke(falseGroupWrongSubjectId))
                                        && (!f.Compile().Invoke(falseGroupWrong)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert(It.Is<Group>(g => (g.Id == trueGroup.Id) && (g.Course == trueGroup.Course))), Times.Never());

            this.groupRepository.Verify(gr => gr.Save(), Times.Never());

        }

        [TestMethod]
        public void ShouldAssertCorrectSemesterFromMonthWhenUsingParseParseSemesterFromMonth()
        {
            const int semester1 = 1;
            const int semester2 = 2;

            const int monthFirstSemester = 4;
            const int monthSecondSemester = 10;

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            Assert.AreEqual(semester1 , action.ParseSemesterFromMonth(monthFirstSemester));
            Assert.AreEqual(semester2, action.ParseSemesterFromMonth(monthSecondSemester));
        }

        private AddGroupToCourseDatabaseEntryAction CreateAddGroupToCourseDatabaseEntryAction()
        {
            return new AddGroupToCourseDatabaseEntryAction(this.courseManagementRepostiories.Object);
        }
    }
}
