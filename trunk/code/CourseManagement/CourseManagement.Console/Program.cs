namespace CourseManagement.Console
{
    using System.Data.Entity;
    using System.Linq;
    using MessageProcessing;
    using MessageProcessing.Actions;
    using MessageProcessing.Rules;
    using Messages;
    using Persistence;
    using Persistence.Initialization;
    using Persistence.Repositories;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

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

            container.Resolve<IGroupFileParser>();

            container.Resolve<BaseRule>("AddDeliverable");
            container.Resolve<BaseRule>("NewGroup");
            container.Resolve<BaseRule>("NewGroup");

            CompositeDatabaseInitializer<CourseManagementContext> compositeDatabaseInitializer = 
                new CompositeDatabaseInitializer<CourseManagementContext>(
                new DropCreateDatabaseIfModelChanges<CourseManagementContext>());

            const string PathToSqlScript = @"Scripts\DbSetup.sql";
            compositeDatabaseInitializer.AddInitializer(new ScriptDataInitializer(PathToSqlScript));

            Database.SetInitializer(compositeDatabaseInitializer);

            // creates the DB));
            using (var db = new CourseManagementContext())
            {
                var s = db.Subjects.FirstOrDefault();
            }
        }
    }
}
