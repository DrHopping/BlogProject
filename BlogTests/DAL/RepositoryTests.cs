using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Mappings;
using BLL.Services;
using Xunit;
using Moq;
using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq.EntityFrameworkCore;

namespace BlogTests.DAL
{
    public class RepositoryTests
    {
        [Fact]
        public void Insert_SuccessfullyCreateBlog()
        {
            //Arrange
            var mockSet = new Mock<DbSet<Blog>>();
            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(m => m.Set<Blog>()).Returns(mockSet.Object);
            var repo = new GenericRepository<Blog>(mockContext.Object);
            var blog = new Blog { Name = "Blog" };
            //Act
            repo.Insert(blog);
            //Assert
            mockSet.Verify(m => m.Add(It.IsAny<Blog>()), Times.Once);
        }

        [Fact]
        public async Task Get_WithNoArguments_ReturnsAllBlogs()
        {
            //Arrange
            var data = new List<Blog>
            {
                new Blog{Name = "BBB"},
                new Blog{Name = "ZZZ"},
                new Blog{Name = "AAA"}
            }.AsQueryable().BuildMockDbSet();
            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(c => c.Set<Blog>()).Returns(data.Object);
            var repo = new GenericRepository<Blog>(mockContext.Object);
            //Act
            var blogs = await repo.GetAllAsync();
            //Assert
            Assert.Equal(3, blogs.Count());
        }

        [Fact]
        public async Task Get_WithFilter_ReturnsSuitableValues()
        {
            //Arrange
            var data = new List<Blog>
            {
                new Blog{Name = "BB"},
                new Blog{Name = "ZZZ"},
                new Blog{Name = "AAAA"}
            }.AsQueryable().BuildMockDbSet();
            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(c => c.Set<Blog>()).Returns(data.Object);
            var repo = new GenericRepository<Blog>(mockContext.Object);
            //Act
            var blogs = await repo.GetAllAsync(b => b.Name.Length > 2);
            Assert.Equal(2, blogs.Count());
        }


    }
}
