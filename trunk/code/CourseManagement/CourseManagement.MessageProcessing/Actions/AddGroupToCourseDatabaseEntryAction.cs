using CourseManagement.Utilities.Extensions;

namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;

    public class AddGroupToCourseDatabaseEntryAction : IAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        private readonly IGroupFileParser groupFileParser;

        public AddGroupToCourseDatabaseEntryAction(ICourseManagementRepositories courseManagementRepositories , IGroupFileParser groupFileParser)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.groupFileParser = groupFileParser;
        }

        public void Execute(IMessage message)
        {
            //verify existing course
            var courseForApplyingGroup = this.GetCourseFromMessage(message);

            //verify existing students
            var studentsIds = this.groupFileParser.ObtainIdsFromMessage(message);
            
            var studentsInCourse = studentsIds.Select(studentId =>
                                                          {
                                                              var student =
                                                                  this.courseManagementRepositories.Students.GetById(
                                                                      studentId);

                                                              if ( student == null )
                                                              {
                                                                  throw new Exception("Students file contains studentsId that don't belong to Student DataBase");
                                                              }

                                                              if (!student.Courses.Contains(courseForApplyingGroup) )
                                                              {
                                                                  throw new Exception("Students file contains studentsId that don't belong to Course");
                                                              }

                                                              return student;
                                                          }).ToList();

            // verify existing group

            var newGroup = new Group { Course = courseForApplyingGroup };

            var groupsInCourse = this.courseManagementRepositories.Groups.Get(g => (g.CourseId == newGroup.CourseId)).ToList();

            studentsInCourse.Select(student =>
                                        {
                                            if (groupsInCourse.Any(groupInCourse => (groupInCourse.Students!=null)&&(groupInCourse.Students.Contains(student))))
                                            {
                                                throw new Exception(
                                                    "A Student has been already assigned for an existing Group in the Course");
                                            }
                                            return student;
                                        }).ToList();

            // Non-existing group. Proceed to add
            newGroup.Students = new List<Student>();

            foreach (var studentToAdd in studentsInCourse)
            {
                newGroup.Students.Add(studentToAdd);
            }

            this.courseManagementRepositories.Groups.Insert(newGroup);

            this.courseManagementRepositories.Groups.Save();

            foreach (var studentAddGroup in studentsInCourse)
            {
                if (studentAddGroup.Groups == null)
                {
                    studentAddGroup.Groups = new List<Group>();
                }

                studentAddGroup.Groups.Add(newGroup);
            }

            this.courseManagementRepositories.Students.Save();
        }

        private Course GetCourseFromMessage(IMessage message)
        {
            int year = message.Date.Year;
            int semester = this.GetSemesterFromMessage(message);

            var courses = this.courseManagementRepositories.Courses.Get(c => (c.Account.User == message.To.First())
                                            && (c.Year == year) && (c.Semester == semester));
            if (courses == null)
            {
                throw new Exception("Course obtained from message doesn't exists");
            }

            return courses.First();
        }

        public int GetSemesterFromMessage(IMessage message)
        {
            return message.Date.Semester();
        }
    }
}
