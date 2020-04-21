using System;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Services;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] UserDTO user)
        {
            try
            {
                var result = await _authService.Authenticate(user);
                if (result == null) throw new ArgumentNullException();
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex);
            }
            catch (WrongCredentialsException ex)
            {
                return NotFound(user);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}