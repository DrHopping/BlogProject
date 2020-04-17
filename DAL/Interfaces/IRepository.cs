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
        void DeleteAndSave(TEntity entity);
        Task DeleteAndSaveAsync(TEntity entity);
        TEntity Update(TEntity entity);
        TEntity UpdateAndSave(TEntity entity);
        Task<TEntity> UpdateAndSaveAsync(TEntity entity);
        TEntity Insert(TEntity entity);
        TEntity InsertAndSave(TEntity entity);
        Task<TEntity> InsertAndSaveAsync(TEntity entity);
        int InsertAndGetId(TEntity entity);
        TEntity GetById(int id, string includeProperties = "");
        Task<TEntity> GetByIdAsync(int id, string includeProperties = "");
        IQueryable<TEntity> GetAllIncluding(string includeProperties = "");
        IQueryable<TEntity> GetAllQueryable();
        IEnumerable<TEntity> GetAll(string includeProperties = "");
        Task<IEnumerable<TEntity>> GetAllAsync(string includeProperties = "");
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");
        TEntity Single(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");
        TEntity FirstOrDefault(int id, string includeProperties = "");
        Task<TEntity> FirstOrDefaultAsync(int id, string includeProperties = "");
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");
        void Dispose();
    }
}