using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;

namespace BLL.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtFactory _jwtFactory;
        private BlogMapper _blogMapper;
        private ArticleMapper _articleMapper;

        public BlogService(IUnitOfWork unitOfWork, IJwtFactory jwtFactory)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
        }

        private BlogMapper BlogMapper => _blogMapper ??= new BlogMapper();
        private ArticleMapper ArticleMapper => _articleMapper ??= new ArticleMapper();

        private bool CheckRights(string token, string id)
        {
            string claimsId = _jwtFactory.GetUserIdClaim(token);
            if (claimsId == null) throw new ArgumentNullException(nameof(claimsId));
            if (id == null) throw new ArgumentNullException(nameof(id));
            return claimsId.CompareTo(id) == 0;
        }

        public async Task<BlogDTO> CreateBlog(BlogDTO blog, string token)
        {
            if (blog == null) throw new ArgumentNullException(nameof(blog));

            var claimsId = _jwtFactory.GetUserIdClaim(token);
            var blogEntity = BlogMapper.Map(blog);
            blogEntity.OwnerId = claimsId;

            _unitOfWork.BlogRepository.Insert(blogEntity);
            await _unitOfWork.SaveAsync();
            blogEntity = (await _unitOfWork.BlogRepository
                .Get(b => b.Name == blog.Name || b.OwnerId == blog.OwnerId))
                .FirstOrDefault();
            if (blogEntity == null) throw new ArgumentNullException(nameof(blogEntity), "Couldn't create blog");
            var result = BlogMapper.Map(blogEntity);
            return result;
        }

        public void DeleteBlog(int id, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var blogEntity = _unitOfWork.BlogRepository.GetById(id);
            if (blogEntity == null) throw new ArgumentNullException(nameof(blogEntity), "This blog doesn't exist");
            if (CheckRights(token, blogEntity.OwnerId))
            {
                _unitOfWork.BlogRepository.Delete(blogEntity);
                _unitOfWork.Save();
            }
            else throw new NotEnoughRightsException();
        }

        public async Task UpdateBlogName(int id, BlogDTO blog, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (blog == null) throw new ArgumentNullException(nameof(blog));
            var entity = _unitOfWork.BlogRepository.GetById(id);
            if (entity == null) throw new ArgumentNullException(nameof(entity), "This blog doesn't exist");
            if (!CheckRights(token, entity.OwnerId)) throw new NotEnoughRightsException();
            if ((await _unitOfWork.BlogRepository.Get(b => b.Name == blog.Name)).FirstOrDefault() != null) throw new NameAlreadyTakenException();
            entity.Name = blog.Name;
            _unitOfWork.BlogRepository.Update(entity);
            _unitOfWork.Save();
        }

        public async Task<BlogDTO> GetBlogById(int id)
        {
            var blog = (await _unitOfWork.BlogRepository.Get((b => b.BlogId == id), includeProperties: "Articles,Owner")).FirstOrDefault();
            if (blog == null) throw new ArgumentNullException(nameof(blog), "Couldn't find blog by id");
            var dto = BlogMapper.Map(blog);
            if (blog.Articles.Any()) dto.Articles = ArticleMapper.Map(blog.Articles);
            if (blog.Owner != null) dto.OwnerUsername = blog.Owner.UserName;
            return dto;
        }

        public async Task<IEnumerable<BlogDTO>> GetAllBlogs()
        {
            return BlogMapper.Map(await _unitOfWork.BlogRepository.Get());
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllArticlesByBlogId(int id)
        {
            var blog = await GetBlogById(id);
            return blog.Articles;
        }
    }
}