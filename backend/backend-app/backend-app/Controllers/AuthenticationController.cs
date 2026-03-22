using Microsoft.AspNetCore.Mvc;
using smartstock_inventory_service.Services;

namespace backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController(AuthenticationService authenticationService) : ControllerBase
    {
        [HttpGet("name")] //http method type is passed here. In hte paranthesis, pass the route i.e ("route/you/want")
        public async Task<ActionResult> GetName()
        {
            /*
             * use method aliases like OK to inform the http server to return an http 200
             if you need dto validation, consider the following model state validation
             
             if (!ModelState.IsValid) return BadRequest(ModelState);
             */
            return Ok(await authenticationService.GetName());
        }
    }
}
