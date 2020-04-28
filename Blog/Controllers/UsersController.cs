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
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public UsersController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _accountService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _accountService.GetUserById(id, Request.GetToken());
            return Ok(user);
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateModel model)
        {
            await _accountService.UpdateUser(id, _mapper.Map<UserDTO>(model), Request.GetToken());
            return NoContent();
        }

        [Authorize]
        [HttpPut]
        [Route("{id}/password")]
        public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] PasswordDTO model)
        {
            await _accountService.ChangePassword(id, model, Request.GetToken());
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _accountService.DeleteUser(id, Request.GetToken());
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterRegularUser([FromBody] RegisterModel registerModel)
        {
            var result = await _accountService.RegisterRegularUser(_mapper.Map<UserDTO>(registerModel));
            return CreatedAtAction(nameof(GetUserById), new
            {
                id = result.Id
            }, result);
        }
    }
}