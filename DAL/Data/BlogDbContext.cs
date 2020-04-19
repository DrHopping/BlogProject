using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class BlogDbContext : IdentityDbContext<User, IdentityRole<int>, int,
        IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        public BlogDbContext() : base() { }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .Property(b => b.LastUpdated)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Article>()
                .Property(b => b.LastUpdated)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Blog>()
                .HasOne<User>(b => b.Owner)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasOne<Blog>(a => a.Blog)
                .WithMany(b => b.Articles)
                .HasForeignKey(a => a.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne<Article>(c => c.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne<User>(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ArticleTag>()
                .HasKey(at => new { at.ArticleId, at.TagId });
            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Article)
                .WithMany(p => p.ArticleTags)
                .HasForeignKey(at => at.TagId);
            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Tag)
                .WithMany(p => p.ArticleTags)
                .HasForeignKey(at => at.ArticleId);
        }
    }
}