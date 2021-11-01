using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core5ApiBoilerplate.Utility.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Core5ApiBoilerplate.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly IUnitOfWork _uow;

        public Repository(IUnitOfWork uow)
        {
            uow.EnsureNotNull();
            _uow = uow;
        }

        public DbSet<T> Set()
        {
            return _uow.Context.Set<T>();
        }

        public T GetById(int id)
        {
            return Query().SingleOrDefault(t => t.Oid == id);
        }

        public IEnumerable<T> GetAll()
        {
            return Set().ToArray();
        }

        public int Count(Expression<Func<T, bool>> filterExpression = null)
        {
            return filterExpression == null 
                ? Set().Count() 
                : Set().Where(filterExpression).Count();
        }

        public void Add(T obj)
        {
            Set().Add(obj);
        }

        public void AddRange(IEnumerable<T> objs)
        {
            var set = Set();
            set.AddRange(objs);
        }

        public void AddOrUpdate(params T[] entities)
        {
            // TODO: Figure out how to do this in EFCore
            // Set().AddOrUpdate(entities);
        }

        public void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities)
        {
            // TODO: Figure out how to do this in EFCore
            // _uow.Context.Set<T>().AddOrUpdate(identifierExpression, entities);
        }

        public void Remove(T obj)
        {
            Set().Remove(obj);
        }

        public void Remove(IEnumerable<T> objs)
        {
            Set().RemoveRange(objs);
        }

        public IQueryable<T> Query()
        {
            return Set().AsQueryable();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> filter)
        {
            return Query().Where(filter);
        }

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return Query().Include(path);
        }
    }
}
