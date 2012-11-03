using System;
using Pop3.Moles;

namespace CourseManagement.Messages.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PopMessageReceiverFixture
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [HostType("Moles")]
        public void ShouldNotBeAbleToConnectWithoutDisconnecting()
        {
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            var messageReceiver = this.CreateMessageReceiver();

            MPop3MailClient.AllInstances.Connect = client => { };

            messageReceiver.Connect(Endpoint, Port, useSsl, User, Password);
            messageReceiver.Connect(Endpoint, Port, useSsl, User, Password);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldConnectToClientAndSetTimeout()
        {
            // arrange
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            var messageReceiver = this.CreateMessageReceiver();

            MPop3MimeClient.ConstructorStringInt32BooleanStringString = 
                (client, endpoint, port, ssl, user, password) =>
                {
                    Assert.AreEqual(Port, port);
                    Assert.AreEqual(User, user);
                    Assert.AreEqual(Password, password);
                    Assert.AreEqual(useSsl, ssl);
                    Assert.AreEqual(Endpoint, endpoint);
                };

            bool connectedToClient = false;
            MPop3MailClient.AllInstances.Connect = client => { connectedToClient = true; };

            bool timeoutSet = false;
            MPop3MailClient.AllInstances.ReadTimeoutSetInt32 = (client, value) => { timeoutSet = true; };

            // act
            messageReceiver.Connect(Endpoint, Port, useSsl, User, Password);

            // assert
            Assert.IsTrue(connectedToClient);
            Assert.IsTrue(timeoutSet);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldDisconnectFromClientWhenDisconnectingAndBeAbleToConnectAgain()
        {
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            var messageReceiver = this.CreateMessageReceiver();
            bool connected = false;
            MPop3MailClient.AllInstances.Connect = client => { connected = true; };

            MPop3MailClient.AllInstances.Disconnect = client => { connected = false; };

            Assert.IsFalse(connected);

            messageReceiver.Connect(Endpoint, Port, useSsl, User, Password);

            Assert.IsTrue(connected);

            messageReceiver.Disconnect();

            Assert.IsFalse(connected);

            messageReceiver.Connect(Endpoint, Port, useSsl, User, Password);

            Assert.IsTrue(connected);
        }

        private PopMessageReceiver CreateMessageReceiver()
        {
            return new PopMessageReceiver();
        }
    }
}
