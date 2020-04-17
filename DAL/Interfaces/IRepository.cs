using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Entities.Base;

namespace DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        void Delete(TEntity entity);
        TEntity Update(TEntity entity);
        TEntity Insert(TEntity entity);
        int InsertAndGetId(TEntity entity);
        TEntity GetById(int id, params Expression<Func<TEntity, object>>[] propertySelectors);
        Task<TEntity> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] propertySelectors);
        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);
        IQueryable<TEntity> GetAllQueryable();
        IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] propertySelectors);
        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] propertySelectors);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);
        TEntity Single(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);
        TEntity FirstOrDefault(int id, params Expression<Func<TEntity, object>>[] propertySelectors);
        Task<TEntity> FirstOrDefaultAsync(int id, params Expression<Func<TEntity, object>>[] propertySelectors);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);
        void Dispose();
    }
}