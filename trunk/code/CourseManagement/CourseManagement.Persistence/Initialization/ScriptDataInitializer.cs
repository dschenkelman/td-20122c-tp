using System.Linq;

namespace CourseManagement.Persistence.Initialization
{
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    public class ScriptDataInitializer : IDatabaseInitializer<CourseManagementContext>
    {
        private readonly string pathToScript;

        public ScriptDataInitializer(string pathToSqlScripts)
        {
            this.pathToScript = pathToSqlScripts;
        }

        public void InitializeDatabase(CourseManagementContext context)
        {
            if (context.Courses.Any())
            {
                return;
            }

            this.LoadFromScript(context);
        }

        private void LoadFromScript(CourseManagementContext context)
        {
            var sql = File.ReadAllText(this.pathToScript);
            context.Database.ExecuteSqlCommand(sql);
        }
    }
}
