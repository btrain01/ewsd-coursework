using Microsoft.AspNetCore.Diagnostics;
using static System.Net.HttpStatusCode;

namespace backend_app.Configurations
{
    public class ExceptionMapper : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var applicationException = exception;

            httpContext.Response.StatusCode = (int) InternalServerError;
            await httpContext.Response.WriteAsync(exception.GetBaseException().Message);
            return true;
        }
    }
}
