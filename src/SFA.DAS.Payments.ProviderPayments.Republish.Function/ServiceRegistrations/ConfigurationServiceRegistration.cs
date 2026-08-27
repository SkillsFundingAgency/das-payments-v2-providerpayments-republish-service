using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Services;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.ServiceRegistrations;

public static class ConfigurationServiceRegistration
{
    public static IServiceCollection AddConfigurationServices(
        this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IConfigurationHelper, AzureFunctionConfigurationHelper>();

        return services;
    }
}