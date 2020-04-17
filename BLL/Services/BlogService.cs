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
            if (await _unitOfWork.BlogRepository.FirstOrDefaultAsync(b => b.Name == blog.Name) != null) throw new NameAlreadyTakenException();

            var claimsId = _jwtFactory.GetUserIdClaim(token);
            var blogEntity = _mapper.Map<BlogDTO, Blog>(blog);
            blogEntity.OwnerId = claimsId;

            var result = await _unitOfWork.BlogRepository.InsertAndSaveAsync(blogEntity);

            return _mapper.Map<Blog, BlogDTO>(result);
        }

        public async Task DeleteBlog(int id, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            var blogEntity = await _unitOfWork.BlogRepository.GetByIdAsync(id);
            if (!CheckRights(token, blogEntity.OwnerId))
                throw new NotEnoughRightsException();
            await _unitOfWork.BlogRepository.DeleteAndSaveAsync(blogEntity);
        }

        public async Task UpdateBlogName(int id, BlogDTO blog, string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (blog == null) throw new ArgumentNullException(nameof(blog));

            var entity = await _unitOfWork.BlogRepository.GetByIdAsync(id);

            if (!CheckRights(token, entity.OwnerId)) throw new NotEnoughRightsException();
            if (await _unitOfWork.BlogRepository.FirstOrDefaultAsync(b => b.Name == blog.Name) != null) throw new NameAlreadyTakenException();

            entity.Name = blog.Name;
            await _unitOfWork.BlogRepository.UpdateAndSaveAsync(entity);
        }

        public async Task<BlogDTO> GetBlogById(int id)
        {
            var blog = await _unitOfWork.BlogRepository.GetByIdAsync(id, "Articles,Owner");
            return _mapper.Map<Blog, BlogDTO>(blog);
        }

        public async Task<IEnumerable<BlogDTO>> GetAllBlogs()
        {
            return _mapper.Map<IEnumerable<Blog>, IEnumerable<BlogDTO>>(await _unitOfWork.BlogRepository.GetAllAsync());
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllArticlesByBlogId(int id)
        {
            var blog = await GetBlogById(id);
            return blog.Articles;
        }
    }
}