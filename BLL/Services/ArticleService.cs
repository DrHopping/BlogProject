using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

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
                var existingTags = (await _unitOfWork.TagRepository.Get()).Select(t => t.Name);
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
            var articles = await _unitOfWork.ArticleRepository.Get(includeProperties: "Tags");
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
                    await _unitOfWork.TagRepository.Get(t => article.Tags.Select(tDto => tDto.Name).Contains(t.Name));
                articleEntity.Tags = tagsEntities;
            }

            _unitOfWork.ArticleRepository.Insert(articleEntity);
            await _unitOfWork.SaveAsync();

            var result = _mapper.Map<Article, ArticleDTO>((await _unitOfWork.ArticleRepository.Get(a => a.Title == article.Title && a.BlogId == article.BlogId)).FirstOrDefault());
            if (result == null) throw new ArgumentException(nameof(result), $"Couldn't create article");

            return result;
        }

        public async Task DeleteArticle(int id, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var article = _unitOfWork.ArticleRepository.GetById(id);
            if (article == null) throw new ArgumentNullException(nameof(article), $"Couldn't find article with id {id}");
            var ownerId = _unitOfWork.BlogRepository.GetById(article.BlogId).OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId))
            {
                if (!_jwtFactory.GetUserRoleClaim(token).Equals("Moderator")) throw new NotEnoughRightsException();
            }

            _unitOfWork.ArticleRepository.Delete(article);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateArticle(int id, ArticleDTO article, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (article == null) throw new ArgumentNullException(nameof(article));
            var entity = _unitOfWork.ArticleRepository.GetById(id);
            if (entity == null) throw new ArgumentNullException(nameof(article));

            var ownerId = _unitOfWork.BlogRepository.GetById(entity.BlogId).OwnerId;
            var requesterId = _jwtFactory.GetUserIdClaim(token);
            if (!ownerId.Equals(requesterId)) throw new NotEnoughRightsException();

            entity.Title = article.Title;
            entity.Content = article.Content;
            _unitOfWork.ArticleRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        //TODO: Better db usage
        public async Task<ArticleDTO> GetArticleById(int id)
        {
            var article = (await _unitOfWork.ArticleRepository.Get(a => a.ArticleId == id, includeProperties: "Tags,Comments.User,Blog.Owner")).FirstOrDefault();
            if (article == null) throw new ArgumentNullException(nameof(article), $"Couldn't find article with id {id}");
            var result = _mapper.Map<Article, ArticleDTO>(article);

            return result;
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsByArticleId(int id)
        {
            var comments =
                await _unitOfWork.CommentRepository.Get(a => a.ArticleId == id, includeProperties: "User");
            return _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDTO>>(comments);
        }

        public async Task<IEnumerable<TagDTO>> GetTagsByArticleId(int id)
        {
            var article =
                (await _unitOfWork.ArticleRepository.Get(a => a.ArticleId == id, includeProperties: "Tags")).FirstOrDefault();
            if (article == null) throw new ArgumentNullException(nameof(article), $"Couldn't find article with id {id}");
            return _mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(article.Tags);
        }

        public async Task<IEnumerable<ArticleDTO>> GetArticlesByTextFilter(string filter)
        {
            var articles = await _unitOfWork.ArticleRepository.Get(a => a.Content.Contains(filter) || a.Title.Contains(filter));
            if (articles == null) throw new ArgumentNullException(nameof(articles));
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllArticles()
        {
            var articles = await _unitOfWork.ArticleRepository.Get(includeProperties: "Blog,Tags");
            if (articles == null) throw new ArgumentNullException(nameof(articles));
            return _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleDTO>>(articles);
        }
    }
}