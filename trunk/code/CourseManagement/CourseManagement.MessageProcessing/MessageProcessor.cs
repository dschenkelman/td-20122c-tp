using CourseManagement.Utilities.Extensions;

namespace CourseManagement.MessageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Rules;
    using Services;

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
            IEnumerable<BaseRule> rules = this.ruleFactory.CreateRules();

            int semester = DateTime.Now.Semester();
                
            int subjectId = this.configurationService.MonitoredSubjectId;
            List<Course> courses = this.courseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == subjectId &&
                c.Semester == semester).ToList();

            Course course = null;
            if (courses.Count > 0)
            {
                course = courses[0];
            }

            if (course != null)
            {
                Configuration configuration = course.Account.Configurations.Single(
                    cfg => cfg.Protocol.Equals(this.configurationService.IncomingMessageProtocol));

                this.messageReceiver.Connect(
                    configuration.Endpoint, 
                    configuration.Port, 
                    configuration.UseSsl,
                    configuration.Account.User,
                    configuration.Account.Password);

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
