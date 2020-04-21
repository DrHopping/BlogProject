using System;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Services;
using Blog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public UserController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private string AuthInfo()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Split(' ')[1];
            if (accessToken == null) throw new ArgumentNullException("Couldn't get the token user authorized with");
            return accessToken;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _accountService.GetUserById(id, AuthInfo());
                if (user == null) throw new ArgumentNullException(nameof(user));
                return Ok(user);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound();
            }
            catch
            {
                return Forbid();
            }

        }

        [HttpPost]
        [AllowAnonymous]
        private async Task<IActionResult> CreateRegularUser([FromBody] UserDTO user)
        {
            try
            {
                var result = await _accountService.RegisterRegularUser(user);
                if (result != null)
                {
                    return CreatedAtAction(nameof(GetUserById), new
                    {
                        id = result.UserId
                    }, result);
                }
                else throw new ArgumentNullException();
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest();
            }
            catch (EmailAlreadyTakenException ex)
            {
                return BadRequest(ex);
            }
            catch (NameAlreadyTakenException ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
