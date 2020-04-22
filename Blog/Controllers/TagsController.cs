using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Services;
using Blog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;


        public TagsController(ITagService tagService, IMapper mapper)
        {
            _tagService = tagService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            return Ok(await _tagService.GetAllTags());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            return Ok(await _tagService.GetTagById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagCreateModel model)
        {
            var result = await _tagService.CreateTag(_mapper.Map<TagDTO>(model));
            return CreatedAtAction(nameof(GetTagById), new
            {
                id = result.Id
            }, result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] TagCreateModel model)
        {
            await _tagService.UpdateTag(id, _mapper.Map<TagDTO>(model));
            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            await _tagService.DeleteTag(id);
            return NoContent();
        }


    }
}