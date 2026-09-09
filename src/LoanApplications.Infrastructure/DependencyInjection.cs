using Azure.Messaging.ServiceBus;
using LoanApplications.Domain.Interfaces;
using LoanApplications.Domain.Repository;
using LoanApplications.Infrastructure.Background;
using LoanApplications.Infrastructure.Persistence;
using LoanApplications.Infrastructure.Queue;
using LoanApplications.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoanApplications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LoanApplicationDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("LoanApplicationConnection"))
        );

        services.AddSingleton(new ServiceBusClient(configuration.GetConnectionString("ServiceBusConnection")));

        services.AddScoped<ILoanApplicationRepository,LoanApplicationRepository>();
        services.AddScoped<IOutBoxRepository, OutBoxRepository>();
        services.AddScoped<IAzureQueuePublisher, AzureQueuePublisher>();
        
        services.AddHostedService<OutBoxProcessorService>();

        return services;

    }
}