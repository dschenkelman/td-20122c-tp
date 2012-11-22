namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Logging;
    using Persistence.Repositories;
    using Utilities.Extensions;

    internal class AddGroupToCourseDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        private readonly IGroupFileParser groupFileParser;
        private readonly ILogger logger;

        public AddGroupToCourseDatabaseEntryAction(
            ICourseManagementRepositories courseManagementRepositories,
            IGroupFileParser groupFileParser,
            ILogger logger)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.groupFileParser = groupFileParser;
            this.logger = logger;
        }

        public void Initialize(ActionEntry actionEntry)
        {
        }

        public void Execute(IMessage message)
        {
            //verify existing course
            var courseForNewGroup = this.GetCourseFromMessage(message);

            //verify existing students
            var studentsIds = this.groupFileParser.GetIdsFromMessage(message);
            
            var studentsInCourse = studentsIds.Select(studentId =>
                {
                    var student =
                        this.courseManagementRepositories.Students.GetById(
                            studentId);

                    if (student == null)
                    {
                        throw new Exception("Students file contains studentsId that don't belong to Student DataBase");
                    }

                    if (!student.Courses.Any(c => c.Id == courseForNewGroup.Id))
                    {
                        throw new Exception("Students file contains studentsId that don't belong to Course");
                    }

                    return student;
                }).ToList();

            // verify existing group
            var newGroup = new Group { CourseId = courseForNewGroup.Id };

            var groupsInCourse = this.courseManagementRepositories.Groups.Get(g => (g.CourseId == newGroup.CourseId));

            studentsInCourse.ForEach(student =>
                {
                    if (
                        groupsInCourse.Any(
                            groupInCourse =>
                            (groupInCourse.Students != null) &&
                            groupInCourse.Students.Any(s => s.Id == student.Id)))
                    {
                        throw new Exception("A Student has been already assigned for an existing Group in the Course");
                    }
                });

            logger.Log(LogLevel.Information, "Adding Group to Course");
            
            // Non-existing group. Proceed to add
            foreach (var studentAddGroup in studentsInCourse)
            {
                studentAddGroup.Groups.Add(newGroup);
            }

            this.courseManagementRepositories.Students.Save();
        }

        private Course GetCourseFromMessage(IMessage message)
        {
            int year = message.Date.Year;
            int semester = this.GetSemesterFromMessage(message);
            string toAddress = message.To.First();

            var course = this.courseManagementRepositories.Courses.Get(c => (c.Account.User == toAddress)
                                            && (c.Year == year) && (c.Semester == semester)).FirstOrDefault();
            if (course == null)
            {
                throw new Exception("Course obtained from message doesn't exists");
            }

            return course;
        }

        public int GetSemesterFromMessage(IMessage message)
        {
            return message.Date.Semester();
        }
    }
}
