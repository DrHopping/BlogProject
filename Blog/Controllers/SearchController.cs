using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Services;
using Blog.Helpers;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public SearchController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        //TODO: Sort articles by tags count
        [HttpGet]
        public async Task<IActionResult> SearchArticles([FromQuery] string filter, [FromQuery] string tags)
        {
            if (!filter.IsNullOrEmpty() && !tags.IsNullOrEmpty())
            {
                return Ok((await _articleService.GetArticlesByTextFilter(filter))
                            .Intersect(await _articleService.GetArticlesByTags(tags), new ArticleEqualityComparer()));
            }

            return !filter.IsNullOrEmpty() ? Ok(await _articleService.GetArticlesByTextFilter(filter))
                                           : Ok(await _articleService.GetArticlesByTags(tags));
        }
    }
}