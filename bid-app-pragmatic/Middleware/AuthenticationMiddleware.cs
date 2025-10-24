namespace bid_app_pragmatic.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();

            if (path.StartsWith("/account/login") ||
                path.StartsWith("/account/register") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib") ||
                path.StartsWith("/favicon"))
            {
                await _next(context);
                return;
            }

            var userId = context.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _logger.LogWarning($"Unauthorized access attempt to: {path}");
                context.Response.Redirect("/Account/Login");
                return;
            }

            await _next(context);
        }
    }
}
