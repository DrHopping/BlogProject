using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtFactory _jwtFactory;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ArticleService(IUnitOfWork unitOfWork, IJwtFactory jwtFactory, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _userManager = userManager;
            _mapper = mapper;
        }

        private async Task<IEnumerable<int>> CreateTagsAndReturnIds(ArticleDTO articleDto)
        {
            var existingTags = (await _unitOfWork.TagRepository.GetAllAsync()).Select(t => t.Name);
            var tagsToInsert = articleDto.Tags.Select(t => t.Name).Except(existingTags);

            foreach (var tagName in tagsToInsert)
            {
                var tagEntity = _mapper.Map<TagDTO, Tag>(new TagDTO { Name = tagName });
                _unitOfWork.TagRepository.Insert(tagEntity);
            }

            await _unitOfWork.SaveAsync();
            var tagNames = articleDto.Tags.Select(t => t.Name);
            var tagIds = (await _unitOfWork.TagRepository.GetAllAsync(t => tagNames.Contains(t.Name))).Select(t => t.Id);
            return tagIds;

        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesByTags(string tags) //done
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            var tagNames = tags.Split(',');
            var articles = (await _unitOfWork.TagRepository.GetAllAsync(t => tagNames.Contains(t.Name), "ArticleTags.Article"))
                .SelectMany(t => t.ArticleTags.Select(at => at.Article));
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }

        public async Task<ArticleDTO> CreateArticle(ArticleDTO articleDto, string token)
        {
            if (articleDto == null) throw new ArgumentNullException(nameof(articleDto));
            if (articleDto.Title == null) throw new ArgumentNullException(nameof(articleDto.Title));
            if (articleDto.Content == null) throw new ArgumentNullException(nameof(articleDto.Content));

            var ownerId = (await _unitOfWork.BlogRepository.GetByIdAsync(articleDto.BlogId.GetValueOrDefault())).OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId)) throw new NotEnoughRightsException();

            var articleEntity = _mapper.Map<ArticleDTO, Article>(articleDto);
            articleEntity = await _unitOfWork.ArticleRepository.InsertAndSaveAsync(articleEntity);

            var articleId = articleEntity.Id;
            if (articleDto.Tags != null && articleDto.Tags.Any())
            {
                var articleTags = new List<ArticleTag>();
                var tagIds = await CreateTagsAndReturnIds(articleDto);
                foreach (var tagId in tagIds)
                {
                    articleTags.Add(new ArticleTag { ArticleId = articleId, TagId = tagId });
                }

                articleEntity.ArticleTags = articleTags;
                articleEntity = await _unitOfWork.ArticleRepository.UpdateAndSaveAsync(articleEntity);
            }

            return _mapper.Map<Article, ArticleDTO>(articleEntity); ;
        }

        public async Task DeleteArticle(int id, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, "Blog");

            var ownerId = article.Blog.OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId))
            {
                if (!_jwtFactory.GetUserRoleClaim(token).Equals("Moderator")) throw new NotEnoughRightsException();
            }

            await _unitOfWork.ArticleRepository.DeleteAndSaveAsync(article);
        }

        public async Task UpdateArticle(int id, ArticleDTO article, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (article == null) throw new ArgumentNullException(nameof(article));
            var entity = await _unitOfWork.ArticleRepository.GetByIdAsync(id, "Blog");

            var ownerId = entity.Blog.OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId)) throw new NotEnoughRightsException();

            entity.Title = article.Title;
            entity.Content = article.Content;
            entity.LastUpdated = DateTime.Now;
            await _unitOfWork.ArticleRepository.UpdateAndSaveAsync(entity);
        }

        public async Task<ArticleDTO> GetArticleById(int id)
        {
            var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, "Tags,Comments.User,Blog.Owner");
            return _mapper.Map<Article, ArticleDTO>(article);
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsByArticleId(int id)
        {
            var comments =
                await _unitOfWork.CommentRepository.GetAllAsync(a => a.ArticleId == id, "User");
            return _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDTO>>(comments);
        }

        public async Task<IEnumerable<TagDTO>> GetTagsByArticleId(int id)
        {
            var article =
                await _unitOfWork.ArticleRepository.GetByIdAsync(id, "ArticleTags.Tag");
            return _mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(article.ArticleTags.Select(at => at.Tag));
        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesByTextFilter(string filter)
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllAsync(a =>
                a.Content.ToLower().Contains(filter.ToLower())
                || a.Title.ToLower().Contains(filter.ToLower()));

            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllArticles()
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllAsync("Blog,ArticleTags.Tag");
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }
    }
}