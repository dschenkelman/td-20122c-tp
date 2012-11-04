﻿namespace CourseManagement.MessageProcessing.Tests.Rules
{
    using System;
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FallbackRuleFixture
    {
        private Mock<IActionFactory> actionFactory;

        [TestInitialize]
        public void Initialize()
        {
            this.actionFactory = new Mock<IActionFactory>(MockBehavior.Strict);
        }

        [TestMethod]
        public void ShouldMatchEverythingWithoutChecking()
        {
            var fallbackRule = this.CreateFallbackRule();

            Mock<IMessage> message = new Mock<IMessage>(MockBehavior.Strict);
            message.Setup(m => m.Subject).Throws(new InvalidOperationException());
            message.Setup(m => m.Body).Throws(new InvalidOperationException());
            message.Setup(m => m.Attachments).Throws(new InvalidOperationException());
            message.Setup(m => m.From).Throws(new InvalidOperationException());
            message.Setup(m => m.To).Throws(new InvalidOperationException());

            Assert.IsTrue(fallbackRule.IsMatch(message.Object));
            Assert.IsTrue(fallbackRule.IsMatch(null));
        }

        private FallbackRule CreateFallbackRule()
        {
            return new FallbackRule(this.actionFactory.Object);
        }
    }
}