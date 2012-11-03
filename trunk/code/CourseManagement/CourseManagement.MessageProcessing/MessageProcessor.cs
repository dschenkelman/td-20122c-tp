using System;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Model;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing
{
    using Rules;

    public class MessageProcessor
    {
        private readonly ICourseManagementRepositories courseManagementRepositories;
        private readonly IRuleFactory ruleFactory;
        private readonly IConfigurationService configurationService;

        public MessageProcessor(IRuleFactory ruleFactory, ICourseManagementRepositories courseManagementRepositories)
        {
            this.ruleFactory = ruleFactory;
            this.courseManagementRepositories = courseManagementRepositories;
        }

        public void Process()
        {
            List<BaseRule> rules = this.ruleFactory.CreateRules().ToList();

        /*    int semester = 1;
            if (DateTime.Now.Month > 6)
                semester = 2;
            Course course = this.courseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == this.configurationService.MonitoringCourseSubjectId &&
                c.Semester == semester).ToList().First();*/
        }
    }
}
