using Newtonsoft.Json;
using StackExchange.Redis;

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
                when ((ex is RedisConnectionException 
                        || ex is RedisTimeoutException 
                        || ex is JsonSerializationException
                    ))
            {
                // Above exceptions are already logged
                await HandleExceptionAsync(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. See exception details.");

                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            // Log the exception or perform any other custom error handling tasks

            var response = new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An error occurred. Please try again later."
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }

}
