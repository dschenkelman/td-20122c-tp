namespace CourseManagement.Messages.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mail.Moles;
    using System.Net.Moles;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SmtpMessageSenderFixture
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldNotBeAbleToConnectWithoutDisconnecting()
        {
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            var messageSender = this.CreateMessageSender();

            messageSender.Connect(Endpoint, Port, useSsl, User, Password);
            messageSender.Connect(Endpoint, Port, useSsl, User, Password);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldDisposeOfClientWhenDisconnectingAndBeAbleToConnectAgain()
        {
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            var messageSender = this.CreateMessageSender();

            bool clientDisposed = false;

            MSmtpClient.AllInstances.Dispose = client => { clientDisposed = true; };

            messageSender.Connect(Endpoint, Port, useSsl, User, Password);

            Assert.IsFalse(clientDisposed);

            messageSender.Disconnect();

            Assert.IsTrue(clientDisposed);

            messageSender.Connect(Endpoint, Port, useSsl, User, Password);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldCreateSmtpClientWithEndpointAndPortAndLogWithCredentialsWhenConnecting()
        {
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            bool smtpClientCreated = false;
            MSmtpClient.ConstructorStringInt32 = (client, endpoint, port) =>
            {
                smtpClientCreated = true;
                Assert.AreEqual(Endpoint, endpoint);
                Assert.AreEqual(Port, port);
            };

            bool enableSslSet = false;
            MSmtpClient.AllInstances.EnableSslSetBoolean = (client, value) =>
            {
                enableSslSet = true;
                Assert.AreEqual(useSsl, value);
            };

            ICredentialsByHost credentialsByHost = null;

            bool networkCredentialsCreated = false;
            MNetworkCredential.ConstructorStringString = (cred, user, pass) =>
            {
                networkCredentialsCreated = true;
                Assert.AreEqual(User, user);
                Assert.AreEqual(Password, pass);
                credentialsByHost = cred;
            };

            bool credentialsSetToClient = false;
            MSmtpClient.AllInstances.CredentialsSetICredentialsByHost = (client, value) =>
            {
                credentialsSetToClient = true;
                Assert.AreEqual(credentialsByHost, value);
            };

            bool useDefaultCredentialsSet = false;
            MSmtpClient.AllInstances.UseDefaultCredentialsSetBoolean = 
                (client, value) =>
                {
                    useDefaultCredentialsSet = true;
                    Assert.IsFalse(value);
                };

            var messageSender = this.CreateMessageSender();

            messageSender.Connect(Endpoint, Port, useSsl, User, Password);

            Assert.IsTrue(credentialsSetToClient);
            Assert.IsTrue(networkCredentialsCreated);
            Assert.IsTrue(smtpClientCreated);
            Assert.IsTrue(enableSslSet);
            Assert.IsTrue(useDefaultCredentialsSet);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldNotBeAbleToSendIfItIsNotConnected()
        {
            var messageSender = this.CreateMessageSender();

            messageSender.Send(null);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldCopyMessageInformationToEmailWhenSending()
        {
            // arrange
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            const string From = "From@From.com";
            const string To1 = "To1@To.com";
            const string To2 = "To2@To.com";
            const string Subject = "Subject";
            const string Body = "Body";

            var messageSender = this.CreateMessageSender();

            Mock<IMessage> message = new Mock<IMessage>(MockBehavior.Strict);
            message.Setup(m => m.From).Returns(From).Verifiable();
            message.Setup(m => m.To).Returns(new List<string> { To1, To2 }).Verifiable();
            message.Setup(m => m.Body).Returns(Body).Verifiable();
            message.Setup(m => m.Subject).Returns(Subject).Verifiable();

            bool fromSet = false;
            MMailMessage.AllInstances.FromSetMailAddress = 
                (m, address) =>
                {
                    fromSet = true;
                    Assert.AreEqual(From, address.Address); 
                };

            bool bodySet = false;
            MMailMessage.AllInstances.BodySetString =
                (m, body) =>
                {
                    bodySet = true;
                    Assert.AreEqual(Body, body);
                };

            bool subjectSet = false;
            MMailMessage.AllInstances.SubjectSetString =
                (m, subject) =>
                {
                    subjectSet = true;
                    Assert.AreEqual(Subject, subject);
                };

            MailAddressCollection addresses = new MailAddressCollection();

            MMailMessage.AllInstances.ToGet = m => { return addresses; };

            bool messageSent = false;
            MSmtpClient.AllInstances.SendMailMessage = (c, m) => { messageSent = true; };

            // act
            messageSender.Connect(Endpoint, Port, useSsl, User, Password);

            messageSender.Send(message.Object);

            // assert
            message.Verify(m => m.From);
            message.Verify(m => m.To);
            message.Verify(m => m.Body);
            message.Verify(m => m.Subject);

            Assert.AreEqual(To1, addresses[0].Address);
            Assert.AreEqual(To2, addresses[1].Address);

            Assert.IsTrue(messageSent);
            Assert.IsTrue(fromSet);
            Assert.IsTrue(subjectSet);
            Assert.IsTrue(bodySet);
        }

        public SmtpMessageSender CreateMessageSender()
        {
            return new SmtpMessageSender();
        }
    }
}
