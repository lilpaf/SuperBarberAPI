namespace SuperBarber.Middlewares
{
    public class JwtAuthorizationHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthorizationHandlingMiddleware> _logger;

        public JwtAuthorizationHandlingMiddleware(
            RequestDelegate next,
            ILogger<JwtAuthorizationHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
                        


            await _next(context);

        }
    }
}
