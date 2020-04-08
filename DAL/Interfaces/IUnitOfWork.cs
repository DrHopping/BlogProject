using System;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Repositories;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Article> ArticleRepository { get; }
        IRepository<Blog> BlogRepository { get; }
        IRepository<Tag> TagRepository { get; }
        IRepository<Comment> CommentRepository { get; }
        void Save();
        Task SaveAsync();
    }
}