using Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Repository.Extensions
{
    public static class RepositoryConfiguration
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {            
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            // + GENERATED_REPOSITORY_REGISTRATION
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
                        services.AddScoped<ICompanyRepository, CompanyRepository>();
                        services.AddScoped<ICompanyOfficeRepository, CompanyOfficeRepository>();
                        services.AddScoped<IParameterRepository, ParameterRepository>();
                        services.AddScoped<IStandardReferenceRepository, StandardReferenceRepository>();
                        services.AddScoped<IStandardReferenceItemRepository, StandardReferenceItemRepository>();
                        services.AddScoped<IStandardReferenceScopeRepository, StandardReferenceScopeRepository>();
                        services.AddScoped<IStandardReferenceScopeItemRepository, StandardReferenceScopeItemRepository>();
                        services.AddScoped<IAutoNumberRepository, AutoNumberRepository>();
                        services.AddScoped<IHolidayRepository, HolidayRepository>();
                        services.AddScoped<IAutoNumberTemplateRepository, AutoNumberTemplateRepository>();
                        services.AddScoped<IAutoNumberCounterRepository, AutoNumberCounterRepository>();
                        services.AddScoped<IAutoNumberComponentRepository, AutoNumberComponentRepository>();
            // Generate Number Engine
            services.AddScoped<IAutoNumberLogRepository, AutoNumberLogRepository>();
            services.AddScoped<IAutoNumberGenerateRepository, AutoNumberGenerateRepository>();
            services.AddScoped<IDocumentNumberRequestRepository, DocumentNumberRequestRepository>();
            // + GENERATED_REPOSITORY_REGISTRATION_END
        }
    }
}
