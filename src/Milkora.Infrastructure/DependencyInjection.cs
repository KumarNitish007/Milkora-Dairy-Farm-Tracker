using Microsoft.Extensions.DependencyInjection;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;
using Milkora.Infrastructure.Repositories;

namespace Milkora.Infrastructure;

/// <summary>Composition helper for the Infrastructure layer. The API calls this
/// so it never needs to know the concrete repository/connection types.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Connection factory is stateless (just the connection string) => singleton.
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        // Repositories are lightweight; scoped keeps them per-request.
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IMilkLogRepository, MilkLogRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IHealthRepository, HealthRepository>();
        services.AddScoped<IBreedingRepository, BreedingRepository>();
        services.AddScoped<IFeedRepository, FeedRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        return services;
    }
}
