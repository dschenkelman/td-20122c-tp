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
    using Persistence.Repositories;

    [TestClass]
    public class AddNewTicketToDatabaseRuleFixture
    {
        private Mock<IActionFactory> actionFactory;
        private MockRepository mockRepository;
        private Mock<ICourseManagementRepositories> repositories;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.actionFactory = this.mockRepository.Create<IActionFactory>();
            this.repositories = this.mockRepository.Create<ICourseManagementRepositories>();
        }

        [TestMethod]
        public void ShouldCreatePublicRegexIfRuleEntryAdditionalDataIsPublic()
        {
            const string ValidPublicSubject = "[CONSULTA-PUBLICA] MVC";
            const string ValidPrivateSubject = "[CONSULTA-PRIVADA] patrones de diseño";
            RuleEntry ruleEntry = new RuleEntry("rule");
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
            RuleEntry ruleEntry = new RuleEntry("rule");
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
            return new AddNewTicketToDatabaseRule(this.actionFactory.Object, this.repositories.Object);
        }
    }
}
