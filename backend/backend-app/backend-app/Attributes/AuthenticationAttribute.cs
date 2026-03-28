using AutoMapper;
using backend_app.DTOs;
using backend_app.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend_app.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthenticationAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IMapper mapper;
        private readonly AuthenticationService authenticationService;

        public AuthenticationAttribute(IMapper mapper, AuthenticationService authenticationService)
        {
            this.mapper = mapper;
            this.authenticationService = authenticationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var unauthorizationObject = new UnauthorizedObjectResult("Invalid Credentials");
            var token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = unauthorizationObject;
                return;
            }

            var user = mapper.Map<string, UserDTO>(token);

            if (user == null)
            {
                context.Result = unauthorizationObject;
                return;
            }

            var isLoggedIn = await authenticationService.IsLoggedIn(user.Id);

            if (!isLoggedIn) 
                context.Result = unauthorizationObject;
            else 
                await next();
        }
    }
}
