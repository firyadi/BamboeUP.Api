using System;
using Mapster;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Hybrid;
using NLog;
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

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.ConfigureCors(builder.Configuration);
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

AuditConfig.Configure(builder.Services, builder.Configuration);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureVersioning();
builder.Services.AddAuthentication();
builder.Services.ConfigureJWT(builder.Configuration, builder.Environment);
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

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
})
.AddApplicationPart(typeof(Presentation.Shell.Controllers.AuthController).Assembly)
.AddApplicationPart(typeof(BamboeUp.Api.Controllers.BanksController).Assembly);

builder.Services.ConfigureSwagger();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsProduction())
    app.UseHsts();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseResponseCompression();
app.UseApiSecurityHeaders();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<Presentation.Shell.ActionFilters.ParameterContextMiddleware>();

app.UseSwagger();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "BamboeUP.API v1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json", "Custom API v2");
    });

    SwaggerDiagnostics.RunDiagnostics(app);
}

app.Run();
