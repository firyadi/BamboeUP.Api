using System;
using Mapster;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NLog;
using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Services;
using BamboeUp.Api;
using BamboeUp.Api.Extensions;
using Contracts;
using Presentation.Shell.ActionFilters;
using Repository;
using Repository.Extensions;
using Service.Shell.Extensions;
using Service.Modules.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
// using Hangfire;
var builder = WebApplication.CreateBuilder(args);

// Logging config (harus sebelum builder.Build)
LogManager.Setup().LoadConfigurationFromFile(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));

// 1. Konfigurasi dasar dan environment
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// 2. Registrasi konfigurasi custom & DI
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();


builder.Services.ConfigureDapperServices(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureTransactionManager();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureShellServices();
builder.Services.ConfigureModulesServices();
builder.Services.Configure<Shared.AdminRegistry.AdminRegistrySettings>(
    builder.Configuration.GetSection(Shared.AdminRegistry.AdminRegistrySettings.SectionName));
MapsterConfig.RegisterMappings();
builder.Services.AddMapster();



// 3. Audit
AuditConfig.Configure(builder.Services, builder.Configuration);

// 4. Middleware & Tools tambahan
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureVersioning();
builder.Services.AddAuthentication();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// builder.Services.ConfigureHangfire(builder.Configuration);

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
})
.AddApplicationPart(typeof(Presentation.Shell.Controllers.AuthController).Assembly)
.AddApplicationPart(typeof(BamboeUp.Api.Controllers.BanksController).Assembly);

// 5. Swagger
builder.Services.ConfigureSwagger();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();

// 1. Error Handling
// app.ConfigureExceptionHandler(logger);
app.UseExceptionHandler(opt => { });

if (app.Environment.IsProduction())
    app.UseHsts();

// 2. HTTP pipeline dasar
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

// 3. CORS & Routing
app.UseCors("CorsPolicy");

app.UseRateLimiter();

// 4. Security
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<Presentation.Shell.ActionFilters.ParameterContextMiddleware>();
// app.UseHangfireDashboard("/hangfire");

// 5. Swagger (pindahkan ini ke setelah MapControllers)
app.UseSwagger();

// 6. Endpoint mapping
app.MapControllers();

// 7. Swagger UI (dipindah ke sini, setelah MapControllers)
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "BamboeUP.API v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Custom API v2");
});

// (opsional debug)
SwaggerDiagnostics.RunDiagnostics(app);
foreach (var endpoint in app.Services.GetRequiredService<EndpointDataSource>().Endpoints)
{
    Console.WriteLine($"Endpoint: {endpoint.DisplayName}");
}


// 8. Run Local background jobs via Hangfire
//RecurringJob.AddOrUpdate<Service.Contracts.Approval.IApprovalService>(
//    "approval-sla-check",
//    approvalService => approvalService.CheckAndProcessSlaAsync(),
//    Cron.Hourly // Cek SLA setiap jam
//);

app.Run();
