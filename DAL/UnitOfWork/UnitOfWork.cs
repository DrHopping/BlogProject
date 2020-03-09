using System;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        public UnitOfWork(BlogDbContext context) { _context = context; }

        private readonly BlogDbContext _context;
        private GenericRepository<Blog> _blogRepository;
        private GenericRepository<Article> _articleRepository;
        private GenericRepository<Comment> _commentRepository;
        private GenericRepository<Tag> _tagRepository;

        public GenericRepository<Article> ArticleRepository
            => _articleRepository ??= new GenericRepository<Article>(_context);
        public GenericRepository<Blog> BlogRepository
            => _blogRepository ??= new GenericRepository<Blog>(_context);
        public GenericRepository<Comment> CommentRepository
            => _commentRepository ??= new GenericRepository<Comment>(_context);
        public GenericRepository<Tag> TagRepository
            => _tagRepository ??= new GenericRepository<Tag>(_context);

        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}