using System.Linq;

namespace CourseManagement.Console
{
    using EmailProcessing;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;
    using Model;
    using Persistence;
    using Persistence.Repositories;

    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            // Initialize the container with the config file
            container.LoadConfiguration();

            container.Resolve<EmailProcessor>();

            container.Resolve<ICourseManagementRepositories>();

            // creates the DB
            using (var db = new CourseManagementContext())
            {
                var s = db.Subjects.FirstOrDefault();
            }
        }
    }
}
