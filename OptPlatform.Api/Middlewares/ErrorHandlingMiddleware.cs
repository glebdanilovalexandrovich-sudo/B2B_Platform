using System.Diagnostics;


namespace OptPlatform.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger <ErrorHandlingMiddleware> logger) 
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

            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке запроса");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new { error = "Внутренняя ошибка сервера", detail = ex.Message });
            }





        }

    }
}
