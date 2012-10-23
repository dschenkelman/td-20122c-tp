namespace CourseManagement.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IRepository<T> : IDisposable
    {
        IEnumerable<T> Get(Expression<Func<T>> filter);
        
        T GetById(int id);
        
        void Insert(T t);
        
        void Delete(int id);
        
        void Update(T t);

        void Save();
    }

}
