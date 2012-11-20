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
    using Utilities.Extensions;
    using Utilities.Log;

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
            Course course = this.courseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == subjectId &&
                c.Semester == semester).FirstOrDefault();


            if (course == null)
            {
                return;
            }

            Configuration configuration = course.Account.Configurations.First(
                cfg => cfg.Protocol.Equals(this.configurationService.IncomingMessageProtocol));

            this.messageReceiver.Connect(
                configuration.Endpoint, 
                configuration.Port, 
                configuration.UseSsl,
                configuration.Account.User,
                configuration.Account.Password);

            foreach (IMessage message in this.messageReceiver.FetchMessages())
            {
                bool previouslyMatched = false;
                foreach (var rule in rules)
                {
                    if (rule.IsMatch(message, previouslyMatched))
                    {
                        previouslyMatched = true;
                        try
                        {
                            rule.Process(message);
                            Logger.GetInstance().LogProcessedRule(this.configurationService.LogsRootPath,
                                         subjectId + "", rule.Name, message.From, message.Date);
                        }catch(InvalidOperationException e)
                        {
                            Logger.GetInstance().LogInvalidOperation(this.configurationService.LogsRootPath,
                                                                     subjectId + "", rule.Name, e.Message);
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }

            this.messageReceiver.Disconnect();
        }
    }
}
