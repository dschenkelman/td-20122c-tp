namespace CourseManagement.Persistence.Initialization
{
    using System.Collections.Generic;
    using System.Data.Entity;

    public class CompositeDatabaseInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        private readonly List<IDatabaseInitializer<T>> initializers;

        public CompositeDatabaseInitializer(params IDatabaseInitializer<T>[] databaseInitializers)
        {
            this.initializers = new List<IDatabaseInitializer<T>>();
            this.initializers.AddRange(databaseInitializers);
        }

        public void InitializeDatabase(T context)
        {
            foreach (var databaseInitializer in this.initializers)
            {
                try
                {
                    databaseInitializer.InitializeDatabase(context);
                }
                catch
                {
                }
            }
        }

        public CompositeDatabaseInitializer<T> AddInitializer(IDatabaseInitializer<T> databaseInitializer)
        {
            this.initializers.Add(databaseInitializer);
            return this;
        }

        public CompositeDatabaseInitializer<T> RemoveInitializer(IDatabaseInitializer<T> databaseInitializer)
        {
            this.initializers.Remove(databaseInitializer);
            return this;
        }
    }
}
