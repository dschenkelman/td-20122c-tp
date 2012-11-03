namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;

    public class AddGroupToCourseDatabaseEntryAction
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;

        public AddGroupToCourseDatabaseEntryAction(ICourseManagementRepositories courseManagementRepositories)
        {
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public void Execute(IMessage message)
        {
            //verify existing course
            var courseForApplyingGroup = this.GetCourseFromMessage(message);

            //verify existing students
            var studentsIds = ObtainIdsFromFile(message);
            
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
                                                          });

            // verify existing group

            var newGroup = new Group { Course = courseForApplyingGroup };

            var groupsInCourse = this.courseManagementRepositories.Groups.Get(g => (g.CourseId == newGroup.CourseId)).ToList();

            studentsInCourse.Select(student =>
                                        {
                                            if (groupsInCourse.Any(groupInCourse => groupInCourse.Students.Contains(student)))
                                            {
                                                throw new Exception(
                                                    "A Student has been already assigned for an existing Group in the Course");
                                            }

                                            return student;
                                        });

            //Non-existing group. Proceed to add

            foreach (var studentToAdd in studentsInCourse)
            {
                newGroup.Students.Add(studentToAdd);
            }
            this.courseManagementRepositories.Groups.Insert(newGroup);

            this.courseManagementRepositories.Groups.Save();

            
            foreach (var studentAddGroup in studentsInCourse)
            {
                studentAddGroup.Groups.Add(newGroup);
            }
            this.courseManagementRepositories.Students.Save();

        }

        private static IEnumerable<int> ObtainIdsFromFile(IMessage message)
        {
            if (message.Attachments.Count() !=1 )
            {
                throw new Exception("Message for Rule AddGroup doesn't have the unique attachment requirement");
            }

            var lines = message.Attachments.First().RetrieveLines();
            
            return lines.Select(line => Convert.ToInt32(line)).ToList();
        }

        private Course GetCourseFromMessage(IMessage message)
        {
            int subjectCode = this.ParseSubjectCodeFromMessage(message);

            int year = message.Date.Year;
            int semester = this.GetSemesterFromMessage(message);

            var courses = this.courseManagementRepositories.Courses.Get(c => (c.SubjectId == subjectCode)
                                            && (c.Year == year) && (c.Semester == semester));
            if ( courses == null )
            {
                throw new Exception("Course obtained from message doesn't exists");
            }

            return courses.First();
        }

        private int ParseSubjectCodeFromMessage(IMessage message)
        {
            string userAccount = message.To.First();
            
            var accounts = this.courseManagementRepositories.Accounts.Get(a => a.User == userAccount).ToList();
            int subjectCode = accounts.ElementAt(0).CourseCode;


            return subjectCode;
        }

        public int GetSemesterFromMessage(IMessage message)
        {
            return message.Date.Month <= 1 && message.Date.Month <= 6 ? 1 : 2;
        }
    }
}
