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
    public class ArticlesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IArticleService _articleService;

        public ArticlesController(IMapper mapper, IArticleService articleService)
        {
            _mapper = mapper;
            _articleService = articleService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            return Ok(await _articleService.GetArticleById(id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleCreateModel model)
        {
            var result = await _articleService.CreateArticle(_mapper.Map<ArticleDTO>(model), Request.GetToken());
            return CreatedAtAction(nameof(GetArticleById), new
            {
                id = result.Id
            }, result);

        }
    }
}