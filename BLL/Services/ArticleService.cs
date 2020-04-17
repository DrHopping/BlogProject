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

        private async Task CreateTags(ArticleDTO articleDto)
        {
            if (articleDto.Tags != null && articleDto.Tags.Any())
            {
                var existingTags = (await _unitOfWork.TagRepository.GetAllAsync()).Select(t => t.Name);
                var tagsToInsert = articleDto.Tags.Select(t => t.Name).Except(existingTags);

                foreach (var tagName in tagsToInsert)
                {
                    var tagEntity = _mapper.Map<TagDTO, Tag>(new TagDTO { Name = tagName });
                    _unitOfWork.TagRepository.Insert(tagEntity);
                }

                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesByTags(string tags)
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));
            var tagNames = tags.Split(',');
            var articles = await _unitOfWork.ArticleRepository.GetAllAsync("Tags");
            articles = articles.Where(a => !tagNames.Except(a.Tags.Select(t => t.Name)).Any());
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }

        public async Task<ArticleDTO> CreateArticle(ArticleDTO article, string token)
        {
            if (article == null) throw new ArgumentNullException(nameof(article));
            if (article.Title == null) throw new ArgumentNullException(nameof(article.Title));
            if (article.Content == null) throw new ArgumentNullException(nameof(article.Content));

            var ownerId = (await _unitOfWork.BlogRepository.GetByIdAsync(article.BlogId.GetValueOrDefault())).OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId)) throw new NotEnoughRightsException();

            var articleEntity = _mapper.Map<ArticleDTO, Article>(article);
            await CreateTags(article);

            if (article.Tags != null && article.Tags.Any())
            {
                var tagsEntities =
                    await _unitOfWork.TagRepository.GetAllAsync(t => article.Tags.Select(tDto => tDto.Name).Contains(t.Name));
                articleEntity.Tags = tagsEntities;
            }

            var result = await _unitOfWork.ArticleRepository.InsertAndSaveAsync(articleEntity);
            return _mapper.Map<Article, ArticleDTO>(result); ;
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
                await _unitOfWork.ArticleRepository.GetByIdAsync(id, "Tags");
            return _mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(article.Tags);
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
            var articles = await _unitOfWork.ArticleRepository.GetAllAsync("Blog,Tags");
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }
    }
}