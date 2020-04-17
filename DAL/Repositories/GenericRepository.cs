using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BLL.Exceptions;
using Castle.Core.Internal;
using DAL.Entities.Base;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class, IEntity<int>
    {
        private bool _disposed = false;
        private DbContext _context;
        private DbSet<TEntity> _dbSet;
        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public void Delete(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public TEntity Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public TEntity Insert(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        public int InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public TEntity GetById(int id, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var entity = FirstOrDefault(id, propertySelectors);
            return entity ?? throw new EntityNotFoundException(nameof(TEntity), id);
        }

        public async Task<TEntity> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var entity = await FirstOrDefaultAsync(id, propertySelectors);
            return entity ?? throw new EntityNotFoundException(nameof(TEntity), id);
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!propertySelectors.IsNullOrEmpty())
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }
            return query;
        }

        public IQueryable<TEntity> GetAllQueryable()
        {
            return GetAllIncluding();
        }

        public IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return await GetAllIncluding(propertySelectors).ToListAsync();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).Where(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return await GetAllIncluding(propertySelectors).Where(predicate).ToListAsync();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return await GetAllIncluding(propertySelectors).SingleAsync(predicate);
        }

        public TEntity FirstOrDefault(int id, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).FirstOrDefault(entity => entity.Id == id);
        }

        public async Task<TEntity> FirstOrDefaultAsync(int id, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return await GetAllIncluding(propertySelectors).FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return await GetAllIncluding(propertySelectors).FirstOrDefaultAsync(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).FirstOrDefault(predicate);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }
    }
}