using System;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.MessageProcessing.Services;
using CourseManagement.Messages;
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
        private readonly IMessageReceiver messageReceiver;

        public MessageProcessor(IRuleFactory ruleFactory, ICourseManagementRepositories courseManagementRepositories, IConfigurationService configurationService, IMessageReceiver messageReceiver)
        {
            this.ruleFactory = ruleFactory;
            this.courseManagementRepositories = courseManagementRepositories;
            this.configurationService = configurationService;
            this.messageReceiver = messageReceiver;
        }

        public void Process()
        {
            List<BaseRule> rules = this.ruleFactory.CreateRules().ToList();

            int semester = 1;
            if (DateTime.Now.Month > 6)
                semester = 2;
            int subjectId = this.configurationService.MonitoringCourseSubjectId;
            List<Course> courses = this.courseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == subjectId &&
                c.Semester == semester).ToList();

            Course course = null;
            if (courses.Count() > 0)
                course = courses.First();

            if( course != null )
            {
                Configuration configuration = course.Account.Configurations.Single(
                    cfg => cfg.Protocol.Equals(this.configurationService.MonitoringCourseIncomingMessageProtocol));

                this.messageReceiver.Connect(configuration.Endpoint, configuration.Port, configuration.UseSsl,
                                             configuration.Account.User, configuration.Account.Password);

                foreach (IMessage message in this.messageReceiver.FetchMessages())
                {
                    foreach (var rule in rules)
                    {
                        if (rule.IsMatch(message))
                        {
                            rule.Process(message);
                        }
                    }
                }
                this.messageReceiver.Disconnect();
            }
        }
    }
}
