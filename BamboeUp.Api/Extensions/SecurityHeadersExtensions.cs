namespace BamboeUp.Api.Extensions;

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseApiSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
            headers.Remove("Server");
            headers.Remove("X-Powered-By");

            await next();
        });
    }
}
