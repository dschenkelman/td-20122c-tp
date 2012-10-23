namespace CourseManagement.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ICourseManagementContext context;

        private IDbSet<T> dbSet;

        public Repository(ICourseManagementContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<T>();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return this.dbSet.Where(predicate);
        }

        public T GetById(int id)
        {
            return this.dbSet.Find(id);
        }

        public void Insert(T t)
        {
            this.dbSet.Add(t);
        }

        public void Delete(int id)
        {
            T t = this.dbSet.Find(id);
            this.dbSet.Remove(t);
        }

        public void Update(T t)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            this.context.SaveChanges();
        }
    }
}
