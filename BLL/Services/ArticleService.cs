using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using Castle.Core.Internal;
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

        private async Task CreateArticleTagConnection(ArticleDTO articleDto, Article articleEntity)
        {
            if (articleDto.Tags != null && articleDto.Tags.Any())
            {
                var articleTags = new List<ArticleTag>();
                var tagIds = await CreateTagsAndReturnIds(articleDto);
                foreach (var tagId in tagIds)
                {
                    articleTags.Add(new ArticleTag { ArticleId = articleEntity.Id, TagId = tagId });
                }

                articleEntity.ArticleTags = articleTags;
                articleEntity = await _unitOfWork.ArticleRepository.UpdateAndSaveAsync(articleEntity);
            }
        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesByTags(string tags)
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            var tagNames = tags.Split(',');
            var articles = (await _unitOfWork.TagRepository.GetAllAsync(t => tagNames.Contains(t.Name), "ArticleTags.Article"))
                .SelectMany(t => t.ArticleTags.Select(at => at.Article));
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }

        public async Task<ArticleDTO> CreateArticle(ArticleDTO articleDto, string token)
        {
            var blog = await _unitOfWork.BlogRepository.GetByIdAsync(articleDto.BlogId.GetValueOrDefault());
            if (blog == null) throw new EntityNotFoundException(nameof(blog), articleDto.BlogId.GetValueOrDefault());
            var ownerId = blog.OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId)) throw new NotEnoughRightsException();

            var articleEntity = _mapper.Map<ArticleDTO, Article>(articleDto);
            articleEntity = await _unitOfWork.ArticleRepository.InsertAndSaveAsync(articleEntity);

            await CreateArticleTagConnection(articleDto, articleEntity);

            return _mapper.Map<Article, ArticleDTO>(articleEntity); ;
        }

        public async Task DeleteArticle(int id, string token)
        {
            var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, "Blog");
            if (article == null) throw new EntityNotFoundException(nameof(article), id);

            var ownerId = article.Blog.OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId))
            {
                if (!_jwtFactory.GetUserRoleClaim(token).Equals("Moderator")) throw new NotEnoughRightsException();
            }

            await _unitOfWork.ArticleRepository.DeleteAndSaveAsync(article);
        }

        public async Task UpdateArticle(int id, ArticleDTO articleDto, string token)
        {
            var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, "Blog,ArticleTags.Tag");
            if (article == null) throw new EntityNotFoundException(nameof(article), id);

            var ownerId = article.Blog.OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId)) throw new NotEnoughRightsException();

            if (!articleDto.Title.IsNullOrEmpty()) article.Title = articleDto.Title;
            if (!articleDto.Content.IsNullOrEmpty()) article.Content = articleDto.Content;
            await CreateArticleTagConnection(articleDto, article);

            article.LastUpdated = DateTime.Now;
            await _unitOfWork.ArticleRepository.UpdateAndSaveAsync(article);
        }

        public async Task<ArticleDTO> GetArticleById(int id)
        {
            var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, "ArticleTags.Tag,Comments.User,Blog.Owner");
            if (article == null) throw new EntityNotFoundException(nameof(article), id);
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
            if (article == null) throw new EntityNotFoundException(nameof(article), id);
            return _mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(article.ArticleTags.Select(at => at.Tag));
        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesByTextFilter(string filter)
        {
            if (filter == null) filter = string.Empty;
            var articles = await _unitOfWork.ArticleRepository.GetAllAsync(a =>
                    a.Content.ToLower().Contains(filter.ToLower())
                    || a.Title.ToLower().Contains(filter.ToLower()),
                    "ArticleTags.Tag,Blog.Owner");

            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllArticles()
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllAsync("Blog,ArticleTags.Tag");
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }
    }
}