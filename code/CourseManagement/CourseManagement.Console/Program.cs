using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;

namespace CourseManagement.Console
{
    using System.Linq;
    using MessageProcessing;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;
    using Persistence;
    using Persistence.Repositories;
    
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            // Initialize the container with the config file
            container.LoadConfiguration();

            container.Resolve<MessageProcessor>();

            container.Resolve<ICourseManagementRepositories>();

            container.Resolve<IMessageReceiver>();

            container.Resolve<IMessageSender>();

            // creates the DB)
            container.Resolve<IGroupFileParser>();

            // creates the DB))
            using (var db = new CourseManagementContext())
            {
                var s = db.Subjects.FirstOrDefault();
            }
        }
    }
}
