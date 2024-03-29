﻿namespace CourseManagement.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IRepository<T> : IDisposable
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
        
        T GetById(int id);
        
        void Insert(T t);
        
        void Delete(int id);

        void Save();
    }

}
