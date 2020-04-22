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
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentsController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            return Ok(await _commentService.GetAllComments());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            return Ok(await _commentService.GetCommentById(id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateModel model)
        {
            var result = await _commentService.AddComment(_mapper.Map<CommentDTO>(model), Request.GetToken());
            return CreatedAtAction(nameof(GetCommentById), new { id = result.Id }, result);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _commentService.DeleteComment(id, Request.GetToken());
            return NoContent();
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateModel model)
        {
            await _commentService.UpdateComment(id, _mapper.Map<CommentDTO>(model), Request.GetToken());
            return NoContent();
        }
    }
}