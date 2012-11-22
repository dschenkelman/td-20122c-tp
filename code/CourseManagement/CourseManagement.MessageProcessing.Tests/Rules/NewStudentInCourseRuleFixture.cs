namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Logging;
    using Persistence.Repositories;

    [TestClass]
    public class NewStudentInCourseRuleFixture
    {
        private MockRepository mockRepository;
        private Mock<IActionFactory> actionFactory;
        private Mock<ICourseManagementRepositories> courseManagementRepositories;
        private Mock<IRepository<Course>> courseRepository;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.courseManagementRepositories = this.mockRepository.Create<ICourseManagementRepositories>();

            this.courseRepository = this.mockRepository.Create<IRepository<Course>>();
            this.courseManagementRepositories.Setup(cmr => cmr.Courses).Returns(this.courseRepository.Object);
            this.logger = this.mockRepository.Create<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void ShouldMatchMessage()
        {
            // arrange
            const int Semester = 2, Year = 2012, SubjectId = 7510;
            Subject trueSubject = new Subject {Code = SubjectId};
            Subject falseSubject = new Subject {Code = 7543};
            Course trueCourse = new Course(Semester, Year, SubjectId) {Id = 0, Subject = trueSubject},
                   falseCourseWrongYear = new Course(Semester, 2010, SubjectId) {Id = 1, Subject = trueSubject},
                   falseCourseWrongSemester = new Course(1, Year, SubjectId) {Id = 2, Subject = trueSubject},
                   falseCourseWrongSubjectId = new Course(Semester, Year, 7543) {Id = 3, Subject = falseSubject};

            this.courseRepository.Setup(
                cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                    f => f.Compile().Invoke(trueCourse)
                         && !f.Compile().Invoke(falseCourseWrongSemester)
                         && !f.Compile().Invoke(falseCourseWrongYear)
                         && !f.Compile().Invoke(falseCourseWrongSubjectId))))
                .Returns(new List<Course> {trueCourse})
                .Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ALTA-MATERIA-7510]91363-Matias Servetto");
            message.Setup(m => m.From).Returns("servetto.matias@gmail.com");
            message.Setup(m => m.Date).Returns(new DateTime(Year, 7, 5));

            NewStudentInCourseRule rule = CreateRule();
            rule.Initialize(new RuleEntry("NewStudent", "^\\[ALTA-MATERIA-(?<subjectCode>[0-9]+)\\][\\ ]*([0-9]+)-([a-zA-Z\\ ]+[a-zA-Z]+)$"));

            // act
            bool resultado = rule.IsMatch(message.Object, false);

            // validate
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void ShouldNotMatchMessagesWithWrongSubject()
        {
            // arrange
            Mock<IMessage> message1 = this.mockRepository.Create<IMessage>();
            message1.Setup(m => m.Subject).Returns("[ALTA-MATERIA-]91363-Matias Servetto");
            message1.Setup(m => m.From).Returns("servetto.matias@gmail.com");
            
            Mock<IMessage> message2 = this.mockRepository.Create<IMessage>();
            message2.Setup(m => m.Subject).Returns("[ALTA-MATERIA-7510]");
            message2.Setup(m => m.From).Returns("servetto.matias@gmail.com");
            
            Mock<IMessage> message3 = this.mockRepository.Create<IMessage>();
            message3.Setup(m => m.Subject).Returns("[ALTA-MATERIA-7510]Matias Servetto");
            message3.Setup(m => m.From).Returns("servetto.matias@gmail.com");

            Mock<IMessage> message4 = this.mockRepository.Create<IMessage>();
            message4.Setup(m => m.Subject).Returns("[MATERIA-7510]91363-Matias Servetto");
            message4.Setup(m => m.From).Returns("servetto.matias@gmail.com");

            Mock<IMessage> message5 = this.mockRepository.Create<IMessage>();
            message5.Setup(m => m.Subject).Returns("[ALTA-7510]91363-Matias Servetto");
            message5.Setup(m => m.From).Returns("servetto.matias@gmail.com");

            NewStudentInCourseRule rule = CreateRule();
            rule.Initialize(new RuleEntry("NewStudent", "^\\[ALTA-MATERIA-(?<subjectCode>[0-9]+)\\][\\ ]*([0-9]+)-([a-zA-Z\\ ]+[a-zA-Z]+)$"));

            // act and validate
            Assert.IsFalse(rule.IsMatch(message1.Object, false));
            Assert.IsFalse(rule.IsMatch(message2.Object, false));
            Assert.IsFalse(rule.IsMatch(message3.Object, false));
            Assert.IsFalse(rule.IsMatch(message4.Object, false));
            Assert.IsFalse(rule.IsMatch(message5.Object, false));
        }

        [TestMethod]
        public void ShouldNotMatchMessageIfSubjectHasNoCourseAtTheSemester()
        {
            // arrange
            const int Semester = 2, Year = 2012, SubjectId = 7543, WrongSubject = 7510;
            Subject trueSubject = new Subject { Code = SubjectId };
            Subject falseSubject = new Subject { Code = WrongSubject };
            Course trueCourse = new Course(Semester, Year, SubjectId) { Id = 0, Subject = trueSubject },
                   falseCourseWrongYear = new Course(Semester, 2010, SubjectId) { Id = 1, Subject = trueSubject },
                   falseCourseWrongSemester = new Course(1, Year, SubjectId) { Id = 2, Subject = trueSubject },
                   falseCourseWrongSubjectId = new Course(Semester, Year, WrongSubject) { Id = 3, Subject = falseSubject };

            this.courseRepository.Setup(
                cr => cr.Get(It.Is<Expression<Func<Course, bool>>>(
                    f => f.Compile().Invoke(trueCourse)
                         && !f.Compile().Invoke(falseCourseWrongSemester)
                         && !f.Compile().Invoke(falseCourseWrongYear)
                         && !f.Compile().Invoke(falseCourseWrongSubjectId))))
                .Returns(new List<Course>())
                .Verifiable();

            Mock<IMessage> message = this.mockRepository.Create<IMessage>();
            message.Setup(m => m.Subject).Returns("[ALTA-MATERIA-" + SubjectId + "]91363-Matias Servetto");
            message.Setup(m => m.From).Returns("servetto.matias@gmail.com");
            message.Setup(m => m.Date).Returns(new DateTime(Year, 7, 5));

            NewStudentInCourseRule rule = CreateRule();
            rule.Initialize(new RuleEntry("NewStudent", "^\\[ALTA-MATERIA-(?<subjectCode>[0-9]+)\\][\\ ]*([0-9]+)-([a-zA-Z\\ ]+[a-zA-Z]+)$"));

            // act and validate
            Assert.IsFalse(rule.IsMatch(message.Object, false));
        }

        private NewStudentInCourseRule CreateRule()
        {
            return new NewStudentInCourseRule(
                this.courseManagementRepositories.Object,
                this.actionFactory.Object,
                this.logger.Object);
        }
    }
}
