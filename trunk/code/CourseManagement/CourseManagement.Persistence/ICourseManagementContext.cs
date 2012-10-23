namespace CourseManagement.Persistence
{
    using System;
    using System.Data.Entity;
    using Model;

    public interface ICourseManagementContext : IDisposable
    {
        int SaveChanges();

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}