using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;

namespace EagleRock.Gateway
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) 
                when (
                    !(ex is RedisConnectionException 
                        || ex is RedisTimeoutException 
                        || ex is JsonSerializationException
                    ))
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                // Customize the response based on the exception
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("An error occurred. Please try again later.");
            }
        }
    }

}
