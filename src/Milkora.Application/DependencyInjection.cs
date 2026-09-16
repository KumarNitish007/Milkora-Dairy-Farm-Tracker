using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Milkora.Application.Interfaces;
using Milkora.Application.Services;

namespace Milkora.Application;

/// <summary>Composition helper for the Application layer: services + validators.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAnimalService, AnimalService>();
        services.AddScoped<IMilkLogService, MilkLogService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IIncomeService, IncomeService>();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<IBreedingService, BreedingService>();
        services.AddScoped<IFeedService, FeedService>();
        services.AddScoped<IReportService, ReportService>();

        // ML predictive features (free, offline). Analysers are stateless => singletons.
        services.AddSingleton<Milkora.ML.MilkForecaster>();
        services.AddSingleton<Milkora.ML.MilkAnomalyDetector>();
        services.AddSingleton<Milkora.ML.MilkHealthAnalyzer>();
        services.AddScoped<IMlService, MlService>();

        // Register every AbstractValidator<T> in this assembly.
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);

        return services;
    }
}
