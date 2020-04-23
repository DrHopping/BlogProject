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

        /// <summary>
        /// Gets article by id
        /// </summary>
        /// <returns>Article with passed id</returns>
        /// <response code="200">Return article with passed id</response>
        /// <response code="404">If article with passed id not found</response> 
        [Produces("application/json")]
        [ProducesResponseType(typeof(ArticleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            return Ok(await _articleService.GetArticleById(id));
        }

        /// <summary>
        /// Gets all articles
        /// </summary>
        /// <returns>All articles</returns>
        /// <response code="200">Return all articles</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(ArticleDTO), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            return Ok(await _articleService.GetAllArticles());
        }

        /// <summary>
        /// Creates new article
        /// </summary>
        /// <returns>Article with passed id</returns>
        /// <response code="201">Returns newly created article </response>
        /// <response code="404">If article with passed id not found</response> 
        [Produces("application/json")]
        [ProducesResponseType(typeof(ArticleDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Updates article with passed id
        /// </summary>
        /// <response code="204">If article successfully updated</response>
        /// <response code="404">If article with passed id not found</response>
        /// <response code="400">If request body not proper</response> 
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] ArticleUpdateModel model)
        {
            await _articleService.UpdateArticle(id, _mapper.Map<ArticleDTO>(model), Request.GetToken());
            return NoContent();
        }

        /// <summary>
        /// Deletes article with passed id
        /// </summary>
        /// <response code="204">If article successfully deleted</response>
        /// <response code="404">If article with passed id not found</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await _articleService.DeleteArticle(id, Request.GetToken());
            return NoContent();
        }

        /// <summary>
        /// Gets tags of article with passed id
        /// </summary>
        /// <returns>Tags of article with passed id</returns>
        /// <response code="200">Returns tags of article with passed id</response>
        /// <response code="404">If article with passed id not found</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(ArticleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/tags")]
        public async Task<IActionResult> GetArticleTags(int id)
        {
            return Ok(await _articleService.GetTagsByArticleId(id));
        }

        /// <summary>
        /// Gets comments of article with passed id
        /// </summary>
        /// <returns>Comments of article with passed id</returns>
        /// <response code="200">Returns comments of article with passed id</response>
        /// <response code="404">If article with passed id not found</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(ArticleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/comments")]
        public async Task<IActionResult> GetArticleComments(int id)
        {
            return Ok(await _articleService.GetCommentsByArticleId(id));
        }

    }
}