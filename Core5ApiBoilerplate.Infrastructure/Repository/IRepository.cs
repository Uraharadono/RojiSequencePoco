using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Core5ApiBoilerplate.Infrastructure.Repository
{
    public interface IRepository
    {
    }

    public interface IRepository<T> : IRepository where T : class, IEntity
    {
        int Count(Expression<Func<T, bool>> filterExpression = null);

        T GetById(int id);
        IEnumerable<T> GetAll();
        
        void Add(T obj);
        void AddRange(IEnumerable<T> objs);
        void AddOrUpdate(params T[] entities);
        void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities);

        void Remove(T obj);
        void Remove(IEnumerable<T> objs);

        IQueryable<T> Query();
        IQueryable<T> Query(Expression<Func<T, bool>> filter);
        IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path);
        
        DbSet<T> Set();
    }
}