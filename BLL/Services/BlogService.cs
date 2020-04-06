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

namespace BLL.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtFactory _jwtFactory;
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork unitOfWork, IJwtFactory jwtFactory, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _mapper = mapper;
        }

        private bool CheckRights(string token, string id)
        {
            var claimsId = _jwtFactory.GetUserIdClaim(token);
            if (claimsId == null) throw new ArgumentNullException(nameof(claimsId));
            if (id == null) throw new ArgumentNullException(nameof(id));
            return claimsId.Equals(id);
        }

        public async Task<BlogDTO> CreateBlog(BlogDTO blog, string token)
        {
            if (blog == null) throw new ArgumentNullException(nameof(blog));

            var claimsId = _jwtFactory.GetUserIdClaim(token);
            var blogEntity = _mapper.Map<BlogDTO, Blog>(blog);
            blogEntity.OwnerId = claimsId;

            _unitOfWork.BlogRepository.Insert(blogEntity);
            await _unitOfWork.SaveAsync();

            blogEntity = (await _unitOfWork.BlogRepository
                .Get(b => b.Name == blog.Name || b.OwnerId == blog.OwnerId))
                .FirstOrDefault();
            if (blogEntity == null) throw new ArgumentNullException(nameof(blogEntity), "Couldn't create blog");

            var result = _mapper.Map<Blog, BlogDTO>(blogEntity);
            return result;
        }

        public async Task DeleteBlog(int id, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var blogEntity = await _unitOfWork.BlogRepository.GetByIdAsync(id);
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

            var entity = await _unitOfWork.BlogRepository.GetByIdAsync(id);
            if (entity == null) throw new ArgumentNullException(nameof(entity), "This blog doesn't exist");

            if (!CheckRights(token, entity.OwnerId)) throw new NotEnoughRightsException();
            if ((await _unitOfWork.BlogRepository.Get(b => b.Name == blog.Name)).FirstOrDefault() != null) throw new NameAlreadyTakenException();

            entity.Name = blog.Name;
            _unitOfWork.BlogRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task<BlogDTO> GetBlogById(int id)
        {
            var blog = (await _unitOfWork.BlogRepository.Get((b => b.BlogId == id), includeProperties: "Articles,Owner")).FirstOrDefault();
            if (blog == null) throw new ArgumentNullException(nameof(blog), "Couldn't find blog by id");

            var dto = _mapper.Map<Blog, BlogDTO>(blog);
            return dto;
        }

        public async Task<IEnumerable<BlogDTO>> GetAllBlogs()
        {
            return _mapper.Map<IEnumerable<Blog>, IEnumerable<BlogDTO>>(await _unitOfWork.BlogRepository.Get());
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllArticlesByBlogId(int id)
        {
            var blog = await GetBlogById(id);
            return blog.Articles;
        }
    }
}