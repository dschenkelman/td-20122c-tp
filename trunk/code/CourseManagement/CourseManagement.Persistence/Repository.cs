namespace CourseManagement.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class Repository<T> : IRepository<T>
    {
        private readonly ICourseManagementContext context;

        public Repository(ICourseManagementContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(Expression<Func<T>> filter)
        {
            throw new NotImplementedException();
        }

        public T GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Insert(T t)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
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
