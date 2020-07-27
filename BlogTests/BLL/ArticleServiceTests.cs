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
using DAL.UnitOfWork;
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

    }
}
