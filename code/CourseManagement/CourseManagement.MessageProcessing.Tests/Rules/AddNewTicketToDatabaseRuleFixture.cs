using CourseManagement.Persistence.Logging;

namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Moq;
    using Persistence.Repositories;

    [TestClass]
    public class AddNewTicketToDatabaseRuleFixture
    {
        private Mock<IActionFactory> actionFactory;
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> repositories;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
            this.logger = this.mockRepository.Create<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void ShouldCreatePublicRegexIfRuleEntryAdditionalDataIsPublic()
        {
            const string ValidPublicSubject = "[CONSULTA-PUBLICA] MVC";
            const string ValidPrivateSubject = "[CONSULTA-PRIVADA] patrones de diseño";
            RuleEntry ruleEntry = new RuleEntry("AddTicket-Public", "^\\[CONSULTA-PUBLICA\\][a-zA-Z\\ ]*$");
            ruleEntry.AdditionalData.Add("public", "true");

            var rule = this.CreateAddTicketReplyToDatabaseRule();

            rule.Initialize(ruleEntry);

            Mock<IMessage> matchingMessage = this.mockRepository.Create<IMessage>();
            matchingMessage.Setup(m => m.Subject).Returns(ValidPublicSubject);
            
            Mock<IMessage> notMatchingMessage = this.mockRepository.Create<IMessage>();
            notMatchingMessage.Setup(m => m.Subject).Returns(ValidPrivateSubject);

            Assert.IsTrue(rule.IsMatch(matchingMessage.Object, false));
            Assert.IsFalse(rule.IsMatch(notMatchingMessage.Object, false));
        }

        [TestMethod]
        public void ShouldCreatePrivateRegexIfRuleEntryAdditionalDataIsPrivate()
        {
            const string ValidPublicSubject = "[CONSULTA-PUBLICA] MVC";
            const string ValidPrivateSubject = "[CONSULTA-PRIVADA] patrones de diseño";
            RuleEntry ruleEntry = new RuleEntry("AddTicket-Public", "\\[CONSULTA-PRIVADA\\][a-zA-Z\\ ]*");
            ruleEntry.AdditionalData.Add("public", "false");

            var rule = this.CreateAddTicketReplyToDatabaseRule();

            rule.Initialize(ruleEntry);

            Mock<IMessage> matchingMessage = this.mockRepository.Create<IMessage>();
            matchingMessage.Setup(m => m.Subject).Returns(ValidPrivateSubject);

            Mock<IMessage> notMatchingMessage = this.mockRepository.Create<IMessage>();
            notMatchingMessage.Setup(m => m.Subject).Returns(ValidPublicSubject);

            Assert.IsTrue(rule.IsMatch(matchingMessage.Object, false));
            Assert.IsFalse(rule.IsMatch(notMatchingMessage.Object, false));
        }

        private AddNewTicketToDatabaseRule CreateAddTicketReplyToDatabaseRule()
        {
            return new AddNewTicketToDatabaseRule(this.actionFactory.Object, 
                this.repositories.Object,
                this.logger.Object);
        }
    }
}
