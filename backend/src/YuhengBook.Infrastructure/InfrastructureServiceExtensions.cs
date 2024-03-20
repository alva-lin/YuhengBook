using System.Reflection;
using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using MediatR;
using YuhengBook.Infrastructure.Data;
using YuhengBook.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YuhengBook.Core.BookAggregate;
using YuhengBook.Core.Interfaces;
using YuhengBook.Infrastructure.Behaviors;
using YuhengBook.UseCases.BookAggregate;

namespace YuhengBook.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        string environmentName
    )
    {
        services.AddMediatR();

        var connectionString = config.GetConnectionString("YuhengBook");
        Guard.Against.Null(connectionString);
        services.AddApplicationDbContext(connectionString);

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        AddServiceByEnvironment(services, config, logger, environmentName);

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var mediatRAssemblies = new[]
        {
            Assembly.GetAssembly(typeof(Book)),             // Core
            Assembly.GetAssembly(typeof(CreateBookCommand)) // UseCases
        };

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(mediatRAssemblies!);
            cfg.AddOpenBehavior(typeof(MyLoggingBehavior<,>));
        });
        // services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        return services;
    }

    private static IServiceCollection AddServiceByEnvironment(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        string environmentName
    )
    {
        if (Environments.Development == environmentName)
        {
            logger.LogInformation("Development environment detected");
            services.AddScoped<IEmailSender, FakeEmailSender>();
        }
        else
        {
            logger.LogInformation("Production environment detected");
            services.AddScoped<IEmailSender, FakeEmailSender>();
        }

        return services;
    }
}
