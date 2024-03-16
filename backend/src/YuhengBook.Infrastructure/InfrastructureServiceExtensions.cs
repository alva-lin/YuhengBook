using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using YuhengBook.Infrastructure.Data;
using YuhengBook.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace YuhengBook.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger)
    {
        var connectionString = config.GetConnectionString("YuhengBook");
        Guard.Against.Null(connectionString);
        services.AddApplicationDbContext(connectionString);

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        services.Configure<MailserverConfiguration>(config.GetSection("Mailserver"));

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}
