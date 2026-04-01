using backend_app.Attributes;
using backend_app.DTOs;
using backend_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MesssageController(MessageService messageService) : ControllerBase
    {
        [HttpPost("send")]
        [ServiceFilter(typeof(AuthenticationAttribute))]
        public async Task<ActionResult> SendMessage([FromBody] CreateMessageDTO createMessageDTO)
        {
            var message = await messageService.CreateMessage(createMessageDTO);
            
            if (message == null)
                return BadRequest("Message could not be sent");

            return Ok(message);
        }

        [HttpGet("recent")]
        public async Task GetIncomingMessages([FromQuery(Name = "auth")] String userToken, CancellationToken cancellationToken)
        {
            var result = messageService.OpenStream(userToken, cancellationToken);
            
            await result.ExecuteAsync(HttpContext);
        }
    }
}
