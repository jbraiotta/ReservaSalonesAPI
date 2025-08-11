namespace ReservaSalones.API.Middleware;

using Microsoft.Extensions.Configuration;

public class ApiKeyAuthorizationMiddleware
{
    private const string ApiKeyHeaderName = "apiKey";

    private readonly IConfiguration _configuration;
    private readonly RequestDelegate _next;

    public ApiKeyAuthorizationMiddleware(IConfiguration configuration, RequestDelegate next)
    {
        _configuration = configuration;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var apiKeyConfig = _configuration.GetSection("ReservaSalonesApi").GetValue<string>("ApiKey");

        if (apiKeyConfig != apiKey)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next.Invoke(context);
    }
}