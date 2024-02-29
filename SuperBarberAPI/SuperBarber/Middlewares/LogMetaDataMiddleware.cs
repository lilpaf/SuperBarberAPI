using System.Diagnostics;

namespace SuperBarber.Middlewares
{
    public class LogMetaDataMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMetaDataMiddleware> _logger;

        public LogMetaDataMiddleware(
            RequestDelegate next,
            ILogger<LogMetaDataMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stopwatch watch = Stopwatch.StartNew();
            await _next.Invoke(context);
            watch.Stop();

            _logger.LogInformation("Finished executing request '{requestId}' with method '{method}' for path '{requestPath}' for {duration}ms on {executionFinishedUTC} UTC time",
               context.TraceIdentifier, context.Request.Method, context.Request.Path, watch.ElapsedMilliseconds, DateTime.UtcNow);
        }
    }
}
