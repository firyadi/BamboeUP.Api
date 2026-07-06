using Contracts;
using Microsoft.AspNetCore.Http;
using Service.Contracts.Shell;

namespace Presentation.Shell.ActionFilters
{
    public class ParameterContextMiddleware
    {
        private readonly RequestDelegate _next;

        public ParameterContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceShellManager serviceShellManager)
        {
            // Resolve the max record based on the current user's token (handled inside AppParameterManager)
            int maxRecord = await serviceShellManager.AppParameterManager.GetMaxResultRecordAsync();
            ParameterContext.MaxResultRecord = maxRecord;

            await _next(context);
        }
    }
}
