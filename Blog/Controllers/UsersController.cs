using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Services;
using Blog.Extensions;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("promote")]
        public async Task<IActionResult> PromoteUser([FromBody] PromoteModel model)
        {
            await _userService.PromoteUser(model.Id, Request.GetToken());
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("unpromote")]
        public async Task<IActionResult> UnpromoteUser([FromBody] PromoteModel model)
        {
            await _userService.UnpromoteUser(model.Id, Request.GetToken());
            return NoContent();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id, Request.GetToken());
            return Ok(user);
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateModel model)
        {
            await _userService.UpdateUser(id, _mapper.Map<UserDTO>(model), Request.GetToken());
            return NoContent();
        }

        [Authorize]
        [HttpPut]
        [Route("{id}/password")]
        public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] PasswordDTO model)
        {
            await _userService.ChangePassword(id, model, Request.GetToken());
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUser(id, Request.GetToken());
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterRegularUser([FromBody] RegisterModel registerModel)
        {
            var result = await _userService.RegisterRegularUser(_mapper.Map<UserDTO>(registerModel));
            return CreatedAtAction(nameof(GetUserById), new
            {
                id = result.Id
            }, result);
        }
    }
}