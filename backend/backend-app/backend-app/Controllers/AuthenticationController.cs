using backend_app.DTOs;
using Microsoft.AspNetCore.Mvc;
using smartstock_inventory_service.Services;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController(AuthenticationService authenticationService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var token = await authenticationService.Login(request);

            if (token == null) return Unauthorized("Invalid username or password");

            return Ok(token);
        }

        [HttpPost("registration")]
        public async Task<ActionResult> Register([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdResult = await authenticationService.RegisterUser(userDTO);

            return Ok(createdResult);
        }
    }
}