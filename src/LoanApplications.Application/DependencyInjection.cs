using FluentValidation;
using LoanApplications.Application.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LoanApplications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services )
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssemblyContaining<CreateLoanApplicationValidator>();

        return services;
    }
    
}