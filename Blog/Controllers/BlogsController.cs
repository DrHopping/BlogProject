using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Services;
using Blog.Extensions;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IMapper _mapper;


        public BlogsController(IBlogService blogService, IMapper mapper)
        {
            _blogService = blogService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromBody] BlogCreateModel model)
        {
            var blog = _mapper.Map<BlogDTO>(model);
            var result = await _blogService.CreateBlog(blog, Request.GetToken());
            return CreatedAtAction(nameof(GetBlogById), new
            {
                id = result.Id
            }, result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            return Ok(await _blogService.GetBlogById(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            return Ok(await _blogService.GetAllBlogs());
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] BlogCreateModel model)
        {
            await _blogService.UpdateBlog(id, _mapper.Map<BlogDTO>(model), Request.GetToken());
            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await _blogService.DeleteBlog(id, Request.GetToken());
            return NoContent();
        }

        [HttpGet]
        [Route("{id}/articles")]
        public async Task<IActionResult> GetBlogArticles(int id)
        {
            return Ok(await _blogService.GetAllArticlesByBlogId(id));
        }

        [HttpGet]
        [Route("/api/users/{userId}/blogs")]
        public async Task<IActionResult> GetUserBlogs(int userId)
        {
            return Ok(await _blogService.GetAllBlogsByUserId(userId));
        }

    }
}