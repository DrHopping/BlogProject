using System;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Repositories;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<Article> ArticleRepository { get; }
        GenericRepository<Blog> BlogRepository { get; }
        GenericRepository<Tag> TagRepository { get; }
        GenericRepository<Comment> CommentRepository { get; }
        void Save();
        Task SaveAsync();
    }
}