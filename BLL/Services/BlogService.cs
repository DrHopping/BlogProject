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

namespace BLL.Services
{
    public class BlogService : IBlogService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtFactory _jwtFactory;
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork unitOfWork, IJwtFactory jwtFactory, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _mapper = mapper;
            _userManager = userManager;
        }

        private bool CheckRights(string token, int id)
        {
            var claimsId = _jwtFactory.GetUserIdClaim(token);
            return claimsId.Equals(id);
        }

        public async Task<BlogDTO> CreateBlog(BlogDTO blogDto, string token)
        {
            if (blogDto == null) throw new ArgumentNullException(nameof(blogDto));
            if ((await _unitOfWork.BlogRepository.FirstOrDefaultAsync(b => b.Name == blogDto.Name)) != null)
                throw new NameAlreadyTakenException(blogDto.Name);

            var claimsId = _jwtFactory.GetUserIdClaim(token);
            var blogEntity = _mapper.Map<BlogDTO, Blog>(blogDto);
            blogEntity.OwnerId = claimsId;

            var result = await _unitOfWork.BlogRepository.InsertAndSaveAsync(blogEntity);

            return _mapper.Map<Blog, BlogDTO>(result);
        }

        public async Task DeleteBlog(int id, string token)
        {
            var blog = await _unitOfWork.BlogRepository.GetByIdAsync(id);
            if (token == null) throw new ArgumentNullException(nameof(token));

            if (!CheckRights(token, blog.OwnerId))
                throw new NotEnoughRightsException();
            await _unitOfWork.BlogRepository.DeleteAndSaveAsync(blog);
        }

        public async Task<BlogDTO> UpdateBlogName(int id, BlogDTO blogDto, string token)
        {
            var blog = await _unitOfWork.BlogRepository.GetByIdAsync(id);
            if (blog == null) throw new EntityNotFoundException(nameof(blog), id);

            if (!CheckRights(token, blog.OwnerId)) throw new NotEnoughRightsException();
            if (await _unitOfWork.BlogRepository.FirstOrDefaultAsync(b => b.Name == blogDto.Name) != null)
                throw new NameAlreadyTakenException(blogDto.Name);

            blog.Name = blogDto.Name;
            var result = await _unitOfWork.BlogRepository.UpdateAndSaveAsync(blog);
            return _mapper.Map<BlogDTO>(result);
        }

        public async Task<BlogDTO> GetBlogById(int id)
        {
            var blog = await _unitOfWork.BlogRepository.GetByIdAsync(id, "Articles,Owner");
            if (blog == null) throw new EntityNotFoundException(nameof(blog), id);
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

        public async Task<IEnumerable<BlogDTO>> GetAllBlogsByUserId(int id)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) throw new EntityNotFoundException(nameof(userEntity), id);
            var blogs = await _unitOfWork.BlogRepository.GetAllAsync(b => b.OwnerId == id);
            return _mapper.Map<IEnumerable<Blog>, IEnumerable<BlogDTO>>(blogs);
        }
    }
}