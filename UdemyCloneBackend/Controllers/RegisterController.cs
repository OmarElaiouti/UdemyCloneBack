using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UdemyCloneBackend.Models.AuthModel;
using UdemyCloneBackend.Services;

namespace UdemyCloneBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthService _authService;

        public RegisterController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> ResultAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);


            var result = await _authService.Register(model);

            if(!result.isAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

    }
}
