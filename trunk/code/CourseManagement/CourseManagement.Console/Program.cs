namespace CourseManagement.Console
{
    using EmailProcessing.Actions;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            // Initialize the container with the config file
            container.LoadConfiguration();

            container.Resolve<IActionFactory>();
        }
    }
}
