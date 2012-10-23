namespace CourseManagement.Console
{
    using EmailProcessing;
    using Entities;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            // Initialize the container with the config file
            container.LoadConfiguration();

            container.Resolve<EmailProcessor>();

            // creates the DB
            using (var db = new CourseManagementContext())
            {
                var a = db.Subjects;
            }
        }
    }
}
