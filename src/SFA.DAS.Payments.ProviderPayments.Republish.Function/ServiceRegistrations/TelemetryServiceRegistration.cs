using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.ServiceRegistrations;

public static class TelemetryServiceRegistration
{
    public static IServiceCollection AddTelemetryServices(
        this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var configHelper =
                sp.GetRequiredService<IConfigurationHelper>();

            return new TelemetryConfiguration(
                configHelper.GetSetting(
                    "ApplicationInsightsInstrumentationKey"));
        });

        return services;
    }
}