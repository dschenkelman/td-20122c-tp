using System;
using System.IO;
using Pop3;

namespace PopSpike
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isSSL = true;
            int port = 995;
            string username = "coursemanagement2012@gmail.com";
            string password = string.Empty;
            string host = "pop.gmail.com";

            Pop3MimeClient popClient = new Pop3MimeClient(host, port, isSSL, username, password);

            popClient.Trace += Console.WriteLine;
            popClient.ReadTimeout = 60000; //give pop server 60 seconds to answer

            //establish connection
            popClient.Connect();

            //get mailbox stats
            int numberOfMailsInMailbox;
            int mailboxSize;
            popClient.GetMailboxStats(out numberOfMailsInMailbox, out mailboxSize);

            //get at most the xx first emails
            RxMailMessage mm;
            int downloadNumberOfEmails;
            int maxDownloadEmails = 4;
            
            if (numberOfMailsInMailbox < maxDownloadEmails)
            {
                downloadNumberOfEmails = numberOfMailsInMailbox;
            }
            else
            {
                downloadNumberOfEmails = maxDownloadEmails;
            }

            for (int i = 1; i <= downloadNumberOfEmails; i++)
            {
                popClient.GetEmail(i, out mm);

                if (mm == null)
                {
                    Console.WriteLine("Email " + i.ToString() + " cannot be displayed.");
                }
                else
                {
                    foreach (var attachment in mm.Attachments)
                    {
                        var stream = attachment.ContentStream;
                        var name = attachment.Name;

                        byte[] content = new byte[stream.Length];

                        stream.Read(content, 0, (int)stream.Length);

                        Directory.CreateDirectory(mm.Subject);

                        File.WriteAllBytes(Path.Combine(mm.Subject, name), content);
                    }

                    Console.WriteLine(mm.MailStructure());
                }
            }

            ////uncomment the following code if you want to write the raw text of the emails to a file.
            //string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string Email;
            //DemoClient.IsCollectRawEmail = true;
            //string fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Emails From POP3 Server.TXT";
            //using (StreamWriter sw = File.CreateText(fileName)) {
            //  for (int i = 1;i <= maxDownloadEmails;i++) {
            //    sw.WriteLine("Email: " + i.ToString() + "\n===============\n\n");
            //    DemoClient.GetRawEmail(i, out Email);
            //    sw.WriteLine(Email);
            //    sw.WriteLine();
            //  }
            //  sw.Close();
            //}

            //close connection
            popClient.Disconnect();

            Console.WriteLine();
            Console.WriteLine("======== Press Enter to end program");
            Console.ReadLine();

            Console.ReadLine();
        }
    }
}
