using CourseManagement.Persistence.Configuration;
using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Model;
    using Persistence.Repositories;
    using Rules;
    using Utilities.Extensions;

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

        public void Process(ILogger logger)
        {
            logger.Log(LogLevel.Information,"Creating Rules");

            IEnumerable<BaseRule> rules = this.ruleFactory.CreateRules( logger );

            int semester = DateTime.Now.Semester();
                
            int subjectId = this.configurationService.MonitoredSubjectId;
            Course course = this.courseManagementRepositories.Courses.Get(
                c =>
                c.Year == DateTime.Now.Year && c.SubjectId == subjectId &&
                c.Semester == semester).FirstOrDefault();

            
            if (course == null)
            {
                logger.Log(LogLevel.Warning, "No Course Found in Database");

                return;
            }

            Configuration configuration = course.Account.Configurations.First(
                cfg => cfg.Protocol.Equals(this.configurationService.IncomingMessageProtocol));

            logger.Log(LogLevel.Information, "Establishing connection with message server");

            try
            {
                this.messageReceiver.Connect(
                    configuration.Endpoint,
                    configuration.Port,
                    configuration.UseSsl,
                    configuration.Account.User,
                    configuration.Account.Password);
            }catch(Exception e)
            {
                logger.Log(LogLevel.Error,"There was an error while trying to establish a connection with de message server.");
                logger.Log(LogLevel.Error, "\tThe exception message was: "+e.Message);
                return;
            }


            logger.Log(LogLevel.Information, "Fetching messages");

            IEnumerable<IMessage> messagesFetched = null;
            try
            {
                messagesFetched = this.messageReceiver.FetchMessages();
            }catch(Exception e)
            {
                logger.Log(LogLevel.Error,"There was an error while trying to fetch the messages.");
                logger.Log(LogLevel.Error, "\tThe exception message was: " + e.Message);
                return;
            }


            foreach (IMessage message in messagesFetched)
            {
                var previouslyMatched = false;
                foreach (var rule in rules)
                {
                    if (rule.IsMatch(message, previouslyMatched))
                    {
                        previouslyMatched = true;
                        try
                        {
                            logger.Log(LogLevel.Information,"Message matched with Rule: "+rule.Name);
                            rule.Process(message,logger);
                        }catch(InvalidOperationException e)
                        {
                            logger.Log(LogLevel.Error, "An error has occured wile processing the message with the rule "+rule.Name);
                            logger.Log(LogLevel.Error, "\tThe exception message was " + e.Message);
                        }
                    }
                }
            }

            this.messageReceiver.Disconnect();
        }
    }
}
