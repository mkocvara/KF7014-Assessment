namespace TemperatureAPI
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, string> _apiKeys;

        public ApiKeyMiddleware(RequestDelegate next, Dictionary<string, string> apiKeys)
        {
            _next = next;
            _apiKeys = apiKeys;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if the request is for the specific route you want to secure
            if (context.Request.Path.StartsWithSegments("/api/Admin"))
            {
                // Extract API key from request headers
                string apiKey = context.Request.Headers["ApiKeyProvid"];

                // Check if the API key is valid
                if (_apiKeys.ContainsValue(apiKey))
                {
                    // API key is valid, proceed to the next middleware
                    await _next(context);
                    return;
                }
                else
                {
                    // API key is invalid, return unauthorized response
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized - Invalid API key");
                    return;
                }
            }

            // If not the secured route, proceed to the next middleware
            await _next(context);
        }
    }
}
