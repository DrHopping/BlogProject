using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class ArticleService
    {
        private IUnitOfWork _unitOfWork;
        private IJwtFactory _jwtFactory;
        private UserManager<User> _userManager;
        private ArticleMapper _articleMapper;
        private TagMapper _tagMapper;
        private CommentMapper _commentMapper;

        public ArticleService(IUnitOfWork unitOfWork, IJwtFactory jwtFactory, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _userManager = userManager;
        }


        private ArticleMapper ArticleMapper => _articleMapper ??= new ArticleMapper();
        private TagMapper TagMapper => _tagMapper ??= new TagMapper();
        private CommentMapper CommentMapper => _commentMapper ??= new CommentMapper();

        private async Task CreateTags(ArticleDTO articleDto)
        {
            if (articleDto.Tags != null && articleDto.Tags.Any())
            {
                foreach (var tag in articleDto.Tags)
                {
                    var tagEntity = (await _unitOfWork.TagRepository.Get(t => t.Name == tag.Name)).FirstOrDefault();
                    if (tagEntity != null) continue;

                    tagEntity = TagMapper.Map(tag);
                    _unitOfWork.TagRepository.Insert(tagEntity);
                    await _unitOfWork.SaveAsync();
                }
            }
        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesByTags(string tags)
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            var tagNames = tags.Split(',');
            var articles = await _unitOfWork.ArticleRepository.Get(includeProperties: "Tags");
            articles = articles.Where(a => tagNames.Except(a.Tags.Select(t => t.Name)).Any());
            return ArticleMapper.Map(articles);
        }

        public async Task<ArticleDTO> CreateArticle(ArticleDTO article, string token)
        {
            if (article == null) throw new ArgumentNullException(nameof(article));
            if (article.Title == null) throw new ArgumentNullException(nameof(article.Title));
            if (article.Content == null) throw new ArgumentNullException(nameof(article.Content));

            var ownerId = (await _unitOfWork.BlogRepository.GetByIdAsync(article.BlogId.GetValueOrDefault())).OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId)) throw new NotEnoughRightsException();

            var articleEntity = ArticleMapper.Map(article);
            await CreateTags(article);

            if (article.Tags != null && article.Tags.Any())
            {
                var tagsEntities =
                    await _unitOfWork.TagRepository.Get(t => article.Tags.Select(tDto => tDto.Name).Contains(t.Name));
                articleEntity.Tags = tagsEntities;
            }

            _unitOfWork.ArticleRepository.Insert(articleEntity);
            await _unitOfWork.SaveAsync();

            var result = ArticleMapper.Map((await _unitOfWork.ArticleRepository.Get(a => a.Title == article.Title && a.BlogId == article.BlogId)).FirstOrDefault());
            if (result == null) throw new ArgumentException(nameof(result), $"Couldn't create article");

            return result;
        }

    }
}