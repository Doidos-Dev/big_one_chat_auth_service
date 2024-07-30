using Data.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Domain.Interfaces;
using Data.Repositories;
using Application.Services.Interfaces;
using Application.Services;

namespace DependencyInjection.Ext
{
    public static class Configurations
    {
        public static void AddConfigurations(this IServiceCollection service, IConfiguration configuration)
        {
            service
                .AddDatabaseConfigurations (configuration)
                .AddRepositories()
                .AddServices();
        }

        public static IServiceCollection AddDatabaseConfigurations(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddEntityFrameworkMongoDB().AddDbContext<DatabaseContext>(options =>
            {
                options.UseMongoDB(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING_DOCKER") ?? configuration["MongoDb:ConnectionStringsLocal"]!,
                    configuration["MongoDb:DbName"]!);
            });

            return service;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection service)
        {
            service.AddScoped<ITokenRepository, TokenRepository>();

            return service;
        }

        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddScoped<IAuthService, AuthService>();

            return service;
        }
    }
}
