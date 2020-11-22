using DogBreedClassification.Api.Commands;
using DogBreedClassification.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogBreedClassification.Api.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
        {
            await _userService.SignUp(command.Email, command.Password, command.FirstName, command.LastName);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
        {
            try
            {
                var jwt = await _userService.SignIn(command.Email, command.Password);
                return Ok(new { Token = jwt });
            }
            catch (ArgumentException)
            {
                return Unauthorized(new { Error = "Invalid credentials"});
            }
        }

        [Authorize(Policy = "User")]
        [HttpGet]
        [Route("api/test")]
        public async Task<IActionResult> Test()
        {
            return Ok("Authorized");
        }

    }
}
