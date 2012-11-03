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
        private Mock<IRepository<Student>> studentRepository;
        private Mock<IMessage> message;
        private Mock<IGroupFileParser> groupParser;

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

            this.studentRepository = this.mockRepository.Create<IRepository<Student>>();
            this.courseManagementRepostiories.Setup(cmr => cmr.Students).Returns(this.studentRepository.Object);

            this.groupParser = this.mockRepository.Create<IGroupFileParser>();
            
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

            this.trueCourse = new Course(this.semester, this.year, this.subjectId){Id = 0};
            this.falseCourseWrongYear = new Course(this.semester, this.wrongYear, this.subjectId) { Id = 1 };
            this.falseCourseWrongSemester = new Course(this.wrongSemester, this.year, this.subjectId) { Id = 2 };
            this.falseCourseWrongSubjectId = new Course(this.semester, this.year, this.wrongSubjectId) { Id = 3 };
            this.falseCourse = new Course(this.wrongSemester, this.wrongYear, this.wrongSubjectId) { Id = 4 };

            this.messageAddress = "sebas@gmail.com";

        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldThrowNonExistingCourseExceptionWhenUsingExecute()
        {
            //arrange
            var courses = new List<Course> {trueCourse};

            trueCourse.Account = new Account{User = "otro"};
            falseCourseWrongYear.Account = new Account { User = this.messageAddress };

            var account = new Account { User = this.messageAddress };
            var accounts = new List<Account> { account };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                                        f.Compile().Invoke(falseCourseWrongYear) && (! f.Compile().Invoke(trueCourse)))))
                                                        .Returns(default(IEnumerable<Course>))
                                                        .Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account)))).Returns(accounts).Verifiable();

            this.message.Setup(e => e.To).Returns(new List<string>{this.messageAddress}).Verifiable();

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

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldThrowNoExistingStudentInDatabaseExceptionWhenUsingExecute()
        {
            //arrange
            var account = new Account { User = this.messageAddress };
            
            trueCourse.Account = new Account { User = this.messageAddress };
            falseCourse.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSemester.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSubjectId.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongYear.Account = new Account { User = "anotherMessageAddres" };

            var accounts = new List<Account> { account };
            
            const int correctId = 90202;
            var studentIds = new List<int> { correctId };

            var courses = new List<Course> { trueCourse };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                            f.Compile().Invoke(trueCourse) && (!f.Compile().Invoke(falseCourse))
                                            && (!f.Compile().Invoke(falseCourseWrongYear)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                            && (!f.Compile().Invoke(falseCourseWrongSemester)))))
                                            .Returns(courses)
                                            .Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account)))).Returns(accounts).Verifiable();

            this.message.Setup(e => e.To).Returns(new List<string> {this.messageAddress}).Verifiable();

            this.message.Setup(e => e.Date).Returns(correctDate).Verifiable();

            this.groupParser.Setup(gp => gp.ObtainIdsFromMessage(this.message.Object)).Returns(studentIds).Verifiable();

            this.studentRepository.Setup(sr => sr.GetById(correctId)).Returns(default(Student)).Verifiable();
            
            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            //act
            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.To, Times.Once());

            this.message.Verify(e => e.Date, Times.Exactly(2));

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                            (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear)))), Times.Once());

            this.groupParser.Verify(gp => gp.ObtainIdsFromMessage(this.message.Object), Times.Once());

            this.studentRepository.Verify(sr => sr.GetById(90202),Times.Once());


        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldThrowNoStudentInCourseExceptionWhenUsingExecute()
        {
            //arrange

            var account = new Account { User = this.messageAddress };
            
            var accounts = new List<Account> { account };
            const int correctId = 90202;
            var studentIds = new List<int> { correctId };

            var coursesOfStudent = new List<Course> {falseCourse, falseCourseWrongSemester};

            trueCourse.Account = new Account { User = this.messageAddress };
            falseCourse.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSemester.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSubjectId.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongYear.Account = new Account { User = "anotherMessageAddres" };


            var studentA = new Student(correctId, "Sebastian", "asd@gmail.com");
            studentA.Courses = coursesOfStudent;

            var correctCourses = new List<Course> { trueCourse };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                            f.Compile().Invoke(trueCourse) && (!f.Compile().Invoke(falseCourse))
                                            && (!f.Compile().Invoke(falseCourseWrongYear)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                            && (!f.Compile().Invoke(falseCourseWrongSemester)))))
                                            .Returns(correctCourses)
                                            .Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account)))).Returns(accounts).Verifiable();

            this.message.Setup(e => e.To).Returns(new List<string> { this.messageAddress }).Verifiable();

            this.message.Setup(e => e.Date).Returns(correctDate).Verifiable();

            this.groupParser.Setup(gp => gp.ObtainIdsFromMessage(this.message.Object)).Returns(studentIds).Verifiable();

            this.studentRepository.Setup(sr => sr.GetById(correctId)).Returns(studentA).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            //act
            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.To, Times.Once());

            this.message.Verify(e => e.Date, Times.Exactly(2));

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                            (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear)))), Times.Once());

            this.groupParser.Verify(gp => gp.ObtainIdsFromMessage(this.message.Object), Times.Once());

            this.studentRepository.Verify(sr => sr.GetById(90202), Times.Once());
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void ShouldThrowStudentInAnExistingGroupInCourseExceptionWhenUsingExecute()
        {
            //arrange

            var account = new Account { User = this.messageAddress};
            var accounts = new List<Account> { account };
            const int correctId = 90202;
            var studentIds = new List<int> { correctId };
            
            var coursesOfStudent = new List<Course> { trueCourse };

            trueCourse.Account = new Account { User = this.messageAddress };
            falseCourse.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSemester.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSubjectId.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongYear.Account = new Account { User = "anotherMessageAddres" };

            var studentA = new Student(correctId, "Sebastian", "asd@gmail.com");
            studentA.Courses = coursesOfStudent;

            var students = new List<Student> {studentA};

            var existingGroup = new Group() { Course = trueCourse, Students = students , CourseId = 0};
            var falseGroup1 = new Group() { Course = falseCourse, Students = students, CourseId = 1};
            var falseGroup2 = new Group() { Course = falseCourseWrongSemester, Students = students, CourseId = 2};
            var falseGroup3 = new Group() { Course = falseCourseWrongSubjectId, Students = students, CourseId = 3};
            var falseGroup4 = new Group() { Course = falseCourseWrongYear, Students = students, CourseId = 4};

            var groupsInCourse = new List<Group> {existingGroup};

            var correctCourses = new List<Course> { trueCourse };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                            f.Compile().Invoke(trueCourse) && (!f.Compile().Invoke(falseCourse))
                                            && (!f.Compile().Invoke(falseCourseWrongYear)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                            && (!f.Compile().Invoke(falseCourseWrongSemester)))))
                                            .Returns(correctCourses)
                                            .Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account)))).Returns(accounts).Verifiable();

            this.message.Setup(e => e.To).Returns(new List<string> { this.messageAddress }).Verifiable();

            this.message.Setup(e => e.Date).Returns(correctDate).Verifiable();

            this.groupParser.Setup(gp => gp.ObtainIdsFromMessage(this.message.Object)).Returns(studentIds).Verifiable();

            this.studentRepository.Setup(sr => sr.GetById(correctId)).Returns(studentA).Verifiable();

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>(f =>
                                            f.Compile().Invoke(existingGroup) && (!f.Compile().Invoke(falseGroup1))
                                            && (!f.Compile().Invoke(falseGroup2)) && (!f.Compile().Invoke(falseGroup3))
                                            && (!f.Compile().Invoke(falseGroup4)))))
                                            .Returns(groupsInCourse)
                                            .Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            //act
            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.To, Times.Once());

            this.message.Verify(e => e.Date, Times.Exactly(2));

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                            (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear)))), Times.Once());

            this.groupParser.Verify(gp => gp.ObtainIdsFromMessage(this.message.Object), Times.Once());

            this.studentRepository.Verify(sr => sr.GetById(90202), Times.Once());

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>(f =>
                                            f.Compile().Invoke(existingGroup) && (!f.Compile().Invoke(falseGroup1))
                                            && (!f.Compile().Invoke(falseGroup2)) && (!f.Compile().Invoke(falseGroup3))
                                            && (!f.Compile().Invoke(falseGroup4)))) , Times.Once() );
        }

        [TestMethod]
        public void ShouldAddNewGroupOfStudentsWhenUsingExecute()
        {
            //arrange

            var account = new Account { User = this.messageAddress };
            var accounts = new List<Account> { account };
            const int correctId1 = 90202;
            
            var studentIds = new List<int> { correctId1 };

            var coursesOfStudent = new List<Course> { trueCourse , falseCourse };

            trueCourse.Account = new Account { User = this.messageAddress };
            falseCourse.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSemester.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongSubjectId.Account = new Account { User = "anotherMessageAddres" };
            falseCourseWrongYear.Account = new Account { User = "anotherMessageAddres" };

            var studentA = new Student(correctId1, "Sebastian", "asd@gmail.com");

            studentA.Courses = coursesOfStudent;

            var students1 = new List<Student> { studentA };

            var trueGroup = new Group() { Course = trueCourse, CourseId = 0 , Students = students1};
            var falseGroup1 = new Group() { Course = falseCourse, CourseId = 1 };
            var falseGroup2 = new Group() { Course = falseCourseWrongSemester, CourseId = 2 };
            var falseGroup3 = new Group() { Course = falseCourseWrongSubjectId, CourseId = 3 };
            var falseGroup4 = new Group() { Course = falseCourseWrongYear, CourseId = 4 };

            var groupsInCourse = new List<Group> { falseGroup1 };

            var correctCourses = new List<Course> { trueCourse };

            this.courseRepository.Setup(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(f =>
                                            f.Compile().Invoke(trueCourse) && (!f.Compile().Invoke(falseCourse))
                                            && (!f.Compile().Invoke(falseCourseWrongYear)) && (!f.Compile().Invoke(falseCourseWrongSubjectId))
                                            && (!f.Compile().Invoke(falseCourseWrongSemester)))))
                                            .Returns(correctCourses)
                                            .Verifiable();

            this.accountRepository.Setup(ar => ar.Get(It.Is<Expression<Func<Account, bool>>>(f => f.Compile().Invoke(account)))).Returns(accounts).Verifiable();

            this.message.Setup(e => e.To).Returns(new List<string> { this.messageAddress }).Verifiable();

            this.message.Setup(e => e.Date).Returns(correctDate).Verifiable();

            this.groupParser.Setup(gp => gp.ObtainIdsFromMessage(this.message.Object)).Returns(studentIds).Verifiable();

            this.studentRepository.Setup(sr => sr.GetById(correctId1)).Returns(studentA).Verifiable();

            this.groupRepository.Setup(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>(f =>
                                            f.Compile().Invoke(trueGroup) && (!f.Compile().Invoke(falseGroup1))
                                            && (!f.Compile().Invoke(falseGroup2)) && (!f.Compile().Invoke(falseGroup3))
                                            && (!f.Compile().Invoke(falseGroup4)))))
                                            .Returns(groupsInCourse)
                                            .Verifiable();

            this.groupRepository.Setup(gr => gr.Insert((It.IsAny<Group>()))).Verifiable();

            this.groupRepository.Setup(gr => gr.Save()).Verifiable();

            this.studentRepository.Setup(sr => sr.Save()).Verifiable();

            AddGroupToCourseDatabaseEntryAction action = this.CreateAddGroupToCourseDatabaseEntryAction();

            //act
            action.Execute(this.message.Object);

            // assert

            this.message.Verify(e => e.To, Times.Exactly(5));

            this.message.Verify(e => e.Date, Times.Exactly(2));

            this.courseRepository.Verify(cr => cr.Get(It.Is<Expression<Func<Course, bool>>>
                            (f => (f.Compile().Invoke(trueCourse)) && (!f.Compile().Invoke(falseCourseWrongYear)))), Times.Once());

            this.groupParser.Verify(gp => gp.ObtainIdsFromMessage(this.message.Object), Times.Once());

            this.studentRepository.Verify(sr => sr.GetById(It.IsAny<int>()), Times.Once());

            this.groupRepository.Verify(gr => gr.Get(It.Is<Expression<Func<Group, bool>>>(f =>
                                f.Compile().Invoke(trueGroup) && (!f.Compile().Invoke(falseGroup1))
                                && (!f.Compile().Invoke(falseGroup2)) && (!f.Compile().Invoke(falseGroup3))
                                && (!f.Compile().Invoke(falseGroup4)))), Times.Once());

            this.groupRepository.Verify(gr => gr.Insert((It.IsAny<Group>())) , Times.Once());

            this.groupRepository.Verify((gr => gr.Save()), Times.Once());

            this.studentRepository.Verify((sr => sr.Save()), Times.Once());

        }

        private AddGroupToCourseDatabaseEntryAction CreateAddGroupToCourseDatabaseEntryAction()
        {
            return new AddGroupToCourseDatabaseEntryAction(this.courseManagementRepostiories.Object , this.groupParser.Object);
        }
    }
}
