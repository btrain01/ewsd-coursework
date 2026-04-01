using backend_app.Attributes;
using backend_app.DTOs;
using backend_app.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(AuthenticationAttribute))]
    public class MesssageController(MessageService messageService) : ControllerBase
    {
        [HttpPost("send")]
        public async Task<ActionResult> SendMessage([FromBody] CreateMessageDTO createMessageDTO)
        {
            var message = await messageService.CreateMessage(createMessageDTO);
            
            if (message == null)
                return BadRequest("Message could not be sent");

            return Ok(message);
        }

        [HttpGet("recent")]
        public async Task GetIncomingMessages(CancellationToken cancellationToken)
        {
            var result = messageService.OpenStream(cancellationToken);
            
            await result.ExecuteAsync(HttpContext);
        }
    }
}
