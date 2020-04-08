using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
<<<<<<< Updated upstream
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
=======
using DAL.Repositories;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
>>>>>>> Stashed changes
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace BlogTests.BLL
{
    public class ArticleServiceTests
    {
        [Theory]
        [InlineData(0, "Space")]
        [InlineData(1, "Bengal")]
        [InlineData(2, "Dog")]
        [InlineData(2, "Hot")]
        [InlineData(2, "Cat")]
        public async Task GetArticlesByTextFilter_ReturnsArticlesThatFitFilter(int count, string filter)
        {
            //Arrange
            var data = new List<Article>
            {
                new Article{ Title = "Cats", Content = "Cats are awesome"},
                new Article{ Title = "Dogs", Content = "Dogs are human friends"},
                new Article{ Title = "HotDog", Content = "Today I going to eat hotdog"},
                new Article{ Title = "Bengal", Content = "Cat with tiger fur"},
                new Article{ Title = "Summer", Content = "This summer is hot"}
            }.AsQueryable().BuildMockDbSet();
            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(c => c.Set<Article>()).Returns(data.Object);
            var service = new ArticleService(new UnitOfWork(mockContext.Object), null, null, MapperProvider.GetMapper());
            //Act
            var filtered = await service.GetArticlesByTextFilter(filter);
            //Assert
            Assert.Equal(count, filtered.Count());
        }

        [Fact]
        public async Task CreateArticle_InsertTags_InsertArticle_ReturnsArticleDtoFromDb()
        {
            //Arrange
            var blogs = new List<Blog>() { new Blog { BlogId = 1, OwnerId = "123" } };
            var mockBlogsDbSet = blogs.AsQueryable().BuildMockDbSet();
            mockBlogsDbSet.Setup(s => s.FindAsync(It.IsAny<int>())).ReturnsAsync(blogs[0]);

            var tags = new List<Tag>()
            {
                new Tag(){Name = "Cat", TagId = 1},
                new Tag(){Name = "Dog", TagId = 2},
                new Tag(){Name = "HotDog", TagId = 3},
            };
            var mockTagsDbSet = tags.AsQueryable().BuildMockDbSet();
            mockTagsDbSet.Setup(s => s.Add(It.IsAny<Tag>())).Callback<Tag>(s => tags.Add(s));

            var articles = new List<Article>();
            var mockArticlesDbSet = articles.AsQueryable().BuildMockDbSet();
            mockArticlesDbSet.Setup(s => s.Add(It.IsAny<Article>())).Callback<Article>(a => articles.Add(a));

            var mockContext = new Mock<BlogDbContext>();
            var mockJwtFactory = new Mock<IJwtFactory>();
            mockJwtFactory.Setup(f => f.GetUserIdClaim(It.IsAny<string>())).Returns("123");
            mockContext.Setup(c => c.Set<Blog>()).Returns(mockBlogsDbSet.Object);
            mockContext.Setup(c => c.Set<Tag>()).Returns(mockTagsDbSet.Object);
            mockContext.Setup(c => c.Set<Article>()).Returns(mockArticlesDbSet.Object);


            var articleDto = new ArticleDTO()
            {
                BlogId = 1,
                Tags = new List<TagDTO>
                {
                    new TagDTO { Name = "Cat" },
                    new TagDTO { Name = "Dog" },
                    new TagDTO { Name = "Friends" },
                    new TagDTO { Name = "Animals"}
                },
                Title = "Friends Forever",
                Content = "Something about cat and dog"
            };

            var mockUof = new Mock<UnitOfWork>(mockContext.Object) { CallBase = true };
            var service = new ArticleService(mockUof.Object, mockJwtFactory.Object, null, MapperProvider.GetMapper());
<<<<<<< Updated upstream
            var result = await service.CreateArticle(articleDto, "test");

=======
            //Act
            var result = await service.CreateArticle(articleDto, "testToken");
            //Assert
>>>>>>> Stashed changes
            Assert.Equal(4, result.Tags.Count());
            Assert.Equal(5, tags.Count);
            mockArticlesDbSet.Verify(s => s.Add(It.IsAny<Article>()), Times.Once);
            mockTagsDbSet.Verify(s => s.Add(It.IsAny<Tag>()), Times.Exactly(2));
            //mockUof.Verify(u => u.TagRepository.Get(null, null, ""), Times.Once);
        }

<<<<<<< Updated upstream

=======
        [Fact]
        public async Task DeleteArticle()
        {
            //Arrange
            var articles = new List<Article>()
            {
                new Article() {ArticleId = 1, Blog = new Blog {OwnerId = "123"}}
            };
            var mockArticlesDbSet = articles.AsQueryable().BuildMockDbSet();

            var mockJwtFactory = new Mock<IJwtFactory>();
            mockJwtFactory.Setup(f => f.GetUserIdClaim(It.IsAny<string>())).Returns("123");

            var mockContext = new Mock<BlogDbContext>();
            mockContext.Setup(c => c.Set<Article>()).Returns(mockArticlesDbSet.Object);

            var mockArticleRepo = new Mock<IRepository<Article>>();
            mockArticleRepo.Setup(r => r.Delete(It.IsAny<Article>())).Callback<Article>(a => articles.Remove(a));
            mockArticleRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Article, bool>>>(),
                    It.IsAny<Func<IQueryable<Article>, IOrderedQueryable<Article>>>(), It.IsAny<string>()))
                .ReturnsAsync(new[] { articles[0] });
            var mockUof = new Mock<IUnitOfWork>();
            mockUof.Setup(s => s.ArticleRepository).Returns(mockArticleRepo.Object);

            var service = new ArticleService(mockUof.Object, mockJwtFactory.Object, null, MapperProvider.GetMapper());

            //Act
            await service.DeleteArticle(1, "testToken");

            //Assert
            Assert.Empty(articles);
        }
>>>>>>> Stashed changes
    }
}
