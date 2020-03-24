using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using DAL.Data;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogTests.DAL
{
    public class RepositoryTests
    {
        [Fact]
        public void Insert_SuccessfullyCreateBlog()
        {
            var mockSet = new Mock<DbSet<Blog>>();

            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(m => m.Set<Blog>()).Returns(mockSet.Object);

            var repo = new GenericRepository<Blog>(mockContext.Object);

            var blog = new Blog()
            {
                Name = "Blog",
            };
            repo.Insert(blog);

            mockSet.Verify(m => m.Add(It.IsAny<Blog>()), Times.Once);
        }

        [Fact]
        public void Get_WithNoArguments_ReturnsAllBlogs()
        {
            var data = new List<Blog>
            {
                new Blog{Name = "BBB"},
                new Blog{Name = "ZZZ"},
                new Blog{Name = "AAA"}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Blog>>();
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(c => c.Set<Blog>()).Returns(mockSet.Object);

            var repo = new GenericRepository<Blog>(mockContext.Object);

            var blogs = repo.Get();

            Assert.Equal(3, blogs.Count());
        }

        [Fact]
        public void Get_WithFilter_ReturnsSuitableValues()
        {
            var data = new List<Blog>
            {
                new Blog{Name = "BB"},
                new Blog{Name = "ZZZ"},
                new Blog{Name = "AAAA"}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Blog>>();
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(c => c.Set<Blog>()).Returns(mockSet.Object);

            var repo = new GenericRepository<Blog>(mockContext.Object);

            var blogs = repo.Get(b => b.Name.Length > 2);

            Assert.Equal(2, blogs.Count());
        }

        [Fact]
        public void Get_WithOrderBy_ReturnsSortedValues()
        {
            var data = new List<Blog>
            {
                new Blog{Name = "BBB"},
                new Blog{Name = "ZZZ"},
                new Blog{Name = "AAA"}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Blog>>();
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(c => c.Set<Blog>()).Returns(mockSet.Object);

            var repo = new GenericRepository<Blog>(mockContext.Object);

            var blogs = repo.Get(orderBy: q => q.OrderBy(b => b.Name));

            Assert.Equal(3, blogs.Count());
            Assert.Equal("AAA", blogs.ElementAt(0).Name);
            Assert.Equal("BBB", blogs.ElementAt(1).Name);
            Assert.Equal("ZZZ", blogs.ElementAt(2).Name);
        }
    }
}
