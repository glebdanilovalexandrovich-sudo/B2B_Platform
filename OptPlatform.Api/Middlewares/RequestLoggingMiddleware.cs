using System.Diagnostics;



namespace OptPlatform.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            var stopWatch = Stopwatch.StartNew(); //start timer

            _logger.LogInformation($"[{DateTime.Now:HH:mm:ss}] {context.Request.Method} {context.Request.Path}"); //write inf 
            await _next(context); //transfer other middl.

            stopWatch.Stop(); //stop timer

            _logger.LogInformation($"{context.Response.StatusCode} {DateTime.Now:HH:mm:ss}");
        }





    }
}
