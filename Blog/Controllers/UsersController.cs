﻿using System;
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
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _accountService.GetUserById(id, Request.GetToken());
            return Ok(user);
        }

        [HttpPost]
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