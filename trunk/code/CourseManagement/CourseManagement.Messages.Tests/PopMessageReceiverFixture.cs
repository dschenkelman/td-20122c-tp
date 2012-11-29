using System.Collections.Generic;
using System.IO;
using System.IO.Moles;
using System.Net.Mail;
using System.Reflection;

namespace CourseManagement.Messages.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Pop3;
    using Pop3.Moles;

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
            MPop3MailClient.AllInstances.Disconnect = client => { };

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
            MPop3MailClient.AllInstances.Disconnect = client => { };

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

        [TestMethod]
        [HostType("Moles")]
        public void ShouldRetrieveStatisticsFromClient()
        {
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            var messageReceiver = this.CreateMessageReceiver();
            MPop3MailClient.AllInstances.Connect = client => { };
            MPop3MailClient.AllInstances.Disconnect = client => { };

            bool retrievedStats = false;
            MPop3MailClient.AllInstances.GetMailboxStatsInt32OutInt32Out = 
                (Pop3MailClient client, out int inboxEmails, out int mailBoxSize) =>
                    { 
                        inboxEmails = 0;
                        mailBoxSize = 0;
                        retrievedStats = true;
                        return true;
                    };

            messageReceiver.Connect(Endpoint, Port, useSsl, User, Password);

            Assert.IsFalse(retrievedStats);

            messageReceiver.FetchMessages().ToList();

            Assert.IsTrue(retrievedStats);
        }

        [TestMethod]
        public void ShouldNotBeAbleToFetchMessagesWithoutConnecting()
        {
            var messageReceiver = this.CreateMessageReceiver();
           
            messageReceiver.FetchMessages();
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldConvertClientMessagesToEmailMessages()
        {
            // arrange
            const string Endpoint = "Endpoint";
            const int Port = 80;
            bool useSsl = false;
            const string User = "User";
            const string Password = "Password";

            var messageReceiver = this.CreateMessageReceiver();
            MPop3MailClient.AllInstances.Connect = client => { };
            MPop3MailClient.AllInstances.Disconnect = client => { };

            MPop3MailClient.AllInstances.GetMailboxStatsInt32OutInt32Out =
                (Pop3MailClient client, out int inboxEmails, out int mailBoxSize) =>
                {
                    inboxEmails = 2;
                    mailBoxSize = 2;
                    return true;
                };

            SRxMailMessage email1 = new SRxMailMessage();
            email1.Subject = "Subject1";
            email1.From = new MailAddress("From1@From.com");
            email1.DeliveryDate = new DateTime(2012, 11, 4);
            email1.To.Add(new MailAddress("To@To.com"));
            var attachment11Stream = new SStream();
            email1.Attachments.Add(new Attachment(attachment11Stream, "Attachment11"));
            var attachment12Stream = new SStream();
            email1.Attachments.Add(new Attachment(attachment12Stream, "Attachment12"));

            SRxMailMessage email2 = new SRxMailMessage();
            email2.Subject = "Subject2";
            email2.From = new MailAddress("From2@From.com");
            email2.DeliveryDate = new DateTime(2012, 11, 5);
            email2.To.Add(new MailAddress("To@To.com"));
            var attachment21Stream = new SStream();
            email2.Attachments.Add(new Attachment(attachment21Stream, "Attachment21"));
            var attachment22Stream = new SStream();
            email2.Attachments.Add(new Attachment(attachment22Stream, "Attachment22"));

            var emails = new Queue<RxMailMessage>();
            emails.Enqueue(email1);
            emails.Enqueue(email2);

            int getEmailCalls = 0;

            MPop3MimeClient.AllInstances.GetEmailInt32RxMailMessageOut =
                (Pop3MimeClient client, int index, out RxMailMessage email) =>
                    {
                        email = emails.Dequeue();
                        getEmailCalls++;
                        return true;
                    };

            // act
            messageReceiver.Connect(Endpoint, Port, useSsl, User, Password);
            
            var retrievedMessages = messageReceiver.FetchMessages().ToList();

            // assert
            Assert.AreEqual(2, getEmailCalls);
            Assert.AreEqual(2, retrievedMessages.Count);

            Assert.AreEqual("Subject1", retrievedMessages[0].Subject);
            Assert.AreEqual("From1@From.com", retrievedMessages[0].From);
            Assert.AreEqual("To@To.com", retrievedMessages[0].To.First());
            Assert.AreEqual(new DateTime(2012, 11, 4), retrievedMessages[0].Date);
            var message1Attachments = retrievedMessages[0].Attachments;
            Assert.AreEqual(2, message1Attachments.Count());
            Assert.AreSame("Attachment11", message1Attachments.ElementAt(0).Name);
            Assert.AreSame("Attachment12", message1Attachments.ElementAt(1).Name);
            Assert.AreSame(
                attachment11Stream,
                this.GetPrivateInstanceField<Func<Stream>>(message1Attachments.ElementAt(0), "getAttachmentStream").Invoke());
            Assert.AreSame(
                attachment12Stream,
                this.GetPrivateInstanceField<Func<Stream>>(message1Attachments.ElementAt(1), "getAttachmentStream").Invoke());

            Assert.AreEqual("Subject2", retrievedMessages[1].Subject);
            Assert.AreEqual("From2@From.com", retrievedMessages[1].From);
            Assert.AreEqual("To@To.com", retrievedMessages[1].To.First());
            Assert.AreEqual(new DateTime(2012, 11, 5), retrievedMessages[1].Date);
            var message2Attachments = retrievedMessages[1].Attachments;
            Assert.AreEqual(2, message2Attachments.Count());
            Assert.AreSame("Attachment21", message2Attachments.ElementAt(0).Name);
            Assert.AreSame("Attachment22", message2Attachments.ElementAt(1).Name);
            Assert.AreSame(
                attachment21Stream,
                this.GetPrivateInstanceField<Func<Stream>>(message2Attachments.ElementAt(0), "getAttachmentStream").Invoke());
            Assert.AreSame(
                attachment22Stream,
                this.GetPrivateInstanceField<Func<Stream>>(message2Attachments.ElementAt(1), "getAttachmentStream").Invoke());
        }

        private T GetPrivateInstanceField<T>(object instance, string fieldName)
        {
            return (T)instance
                .GetType()
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(instance);
        }

        private PopMessageReceiver CreateMessageReceiver()
        {
            return new PopMessageReceiver();
        }
    }
}
