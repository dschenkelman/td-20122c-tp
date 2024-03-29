﻿namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System;
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Persistence.Logging;

    [TestClass]
    public class FallbackRuleFixture
    {
        private Mock<IActionFactory> actionFactory;
        private Mock<ILogger> logger;

        [TestInitialize]
        public void Initialize()
        {
            this.actionFactory = new Mock<IActionFactory>(MockBehavior.Strict);
            this.logger = new Mock<ILogger>();
            this.logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void ShouldMatchEverythingWithoutCheckingIfNotPreviouslyMatched()
        {
            var fallbackRule = this.CreateFallbackRule();

            Mock<IMessage> message = new Mock<IMessage>(MockBehavior.Strict);
            message.Setup(m => m.Subject).Throws(new InvalidOperationException());
            message.Setup(m => m.Body).Throws(new InvalidOperationException());
            message.Setup(m => m.Attachments).Throws(new InvalidOperationException());
            message.Setup(m => m.From).Throws(new InvalidOperationException());
            message.Setup(m => m.To).Throws(new InvalidOperationException());

            Assert.IsTrue(fallbackRule.IsMatch(message.Object, false));
            Assert.IsTrue(fallbackRule.IsMatch(null, false));
        }

        [TestMethod]
        public void ShouldReturnFalseIfAPreviousRuleWasMatched()
        {
            var fallbackRule = this.CreateFallbackRule();

            Mock<IMessage> message = new Mock<IMessage>(MockBehavior.Strict);
            message.Setup(m => m.Body).Throws(new InvalidOperationException());
            message.Setup(m => m.Attachments).Throws(new InvalidOperationException());
            message.Setup(m => m.From).Throws(new InvalidOperationException());
            message.Setup(m => m.To).Throws(new InvalidOperationException());

            Assert.IsFalse(fallbackRule.IsMatch(message.Object, true));
            Assert.IsFalse(fallbackRule.IsMatch(null, true));
        }

        private FallbackRule CreateFallbackRule()
        {
            return new FallbackRule(this.actionFactory.Object, this.logger.Object);
        }
    }
}
