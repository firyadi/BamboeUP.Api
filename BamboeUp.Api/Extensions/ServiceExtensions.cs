using Asp.Versioning;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Service.Contracts.Shell;
using Service.Contracts.Modules;
using Service.Shell;
using Service.Modules;
using Shared.Settings;
using System.Text;
// using Hangfire;

namespace BamboeUp.Api.Extensions
{
    public static class ServiceExtensions
    {

        public static void ConfigureDapperServices(this IServiceCollection services, IConfiguration config)
        {
            // Database
            var sqlConn = config.GetConnectionString("sqlConnection") 
                ?? throw new InvalidOperationException("Connection string 'sqlConnection' not found.");
            services.AddScoped<RepositoryContext>(_ =>
                new RepositoryContext(sqlConn));

            // Repository Services
            services.AddScoped<ITransactionManager, DapperTransactionManager>();
            services.AddScoped<IServiceShellManager, ServiceShellManager>();
            services.AddScoped<IServiceModulesManager, ServiceModulesManager>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserContext, BamboeUp.Api.Context.UserContext>();
            services.AddHttpContextAccessor();

        }

        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>() ?? new CorsSettings();
            var allowedOrigins = corsSettings.AllowedOrigins
                .Where(origin => !string.IsNullOrWhiteSpace(origin))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        builder.WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                    else
                    {
                        builder.SetIsOriginAllowed(_ => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }

                    builder.WithExposedHeaders("X-Pagination");
                });
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
             services.Configure<IISOptions>(options =>
             {
             });

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerManager, LoggerManager>();

        // Extensions/ServiceExtensions.cs
        public static void ConfigureDapperContext(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConn = configuration.GetConnectionString("sqlConnection") 
                ?? throw new InvalidOperationException("Connection string 'sqlConnection' not found.");
            services.AddScoped<RepositoryContext>(_ =>
                new RepositoryContext(sqlConn));
        }

        // Extensions/ServiceExtensions.cs
        public static void ConfigureTransactionManager(this IServiceCollection services)
        {
            services.AddScoped<ITransactionManager, DapperTransactionManager>();

        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            }).AddMvc();
        }


        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtConfiguration>()
                ?? throw new InvalidOperationException("JwtSettings is not configured properly");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !environment.IsDevelopment();
                options.SaveToken = false;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        }


        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
            => services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<ICompanyRepository, CompanyRepository>();
            //services.AddScoped<ICompanyOfficeRepository, CompanyOfficeRepository>();
        }

        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddHangfire(config => config
            //    .SetDataCompatibilityLevel(Hangfire.CompatibilityLevel.Version_180)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseSqlServerStorage(configuration.GetConnectionString("sqlConnection")));

            //services.AddHangfireServer();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BamboeUP.API",
                    Version = "v1",
                    Description = "BamboeUP.API API by Firyadi",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "John Doe",
                        Email = "John.Doe@gmail.com",
                        Url = new Uri("https://twitter.com/johndoe"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "CompanyEmployees API LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                s.SwaggerDoc("v2", new OpenApiInfo { Title = "Custom API", Version = "v2" });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                        },
                        []
                    }
                });
            });
        }
    }
}
