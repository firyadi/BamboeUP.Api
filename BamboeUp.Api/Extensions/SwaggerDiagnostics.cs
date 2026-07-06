using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace BamboeUp.Api.Extensions
{
    public static class SwaggerDiagnostics
    {
        public static void RunDiagnostics(WebApplication app)
        {
            var logger = app.Services.GetService<ILoggerFactory>()?.CreateLogger("SwaggerDiagnostics");
            var apiDescProvider = app.Services.GetService<IApiDescriptionGroupCollectionProvider>();

            if (apiDescProvider == null)
            {
                logger?.LogError("IApiDescriptionGroupCollectionProvider is not available. Swagger might be misconfigured.");
                return;
            }

            var descriptions = apiDescProvider.ApiDescriptionGroups.Items.SelectMany(g => g.Items).ToList();

            if (descriptions.Count == 0)
            {
                logger?.LogWarning("Swagger loaded, but no endpoints were found.");
                logger?.LogWarning("Possible causes:");
                logger?.LogWarning("- No [HttpGet]/[HttpPost] controllers defined.");
                logger?.LogWarning("- Missing app.UseEndpoints or MapControllers().");
                logger?.LogWarning("- Controller classes not in correct namespace or not public.");
                logger?.LogWarning("- Endpoint metadata not exposed to Swagger (e.g. using minimal APIs without annotation).");
            }
            else
            {
                logger?.LogInformation($"Swagger loaded {descriptions.Count} endpoints.");
            }
        }
    }

}
