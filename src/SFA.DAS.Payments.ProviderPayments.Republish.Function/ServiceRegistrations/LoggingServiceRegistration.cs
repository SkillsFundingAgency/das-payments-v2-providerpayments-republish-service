using System;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.ServiceRegistrations;

public static class LoggingServiceRegistration
{
    public static IServiceCollection AddLoggingServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ILoggerConfigurationBuilder,
            PaymentsLoggerConfigurationBuilder>();

        services.AddSingleton<IApplicationLoggerSettings>(sp =>
        {
            var versionInfo = sp.GetRequiredService<IVersionInfo>();
            var configHelper = sp.GetRequiredService<IConfigurationHelper>();

            if (!Enum.TryParse(
                configHelper.GetSettingOrDefault(
                    "LogLevel",
                    "Information"),
                out LogLevel logLevel))
            {
                logLevel = LogLevel.Information;
            }

            return new ApplicationLoggerSettings
            {
                ApplicationLoggerOutputSettingsCollection =
                [
                    new ConsoleApplicationLoggerOutputSettings
                    {
                        MinimumLogLevel = logLevel
                    }
                ],
                TaskKey = versionInfo.ServiceReleaseVersion
            };
        });

        services.AddSingleton<ISerilogLoggerFactory>(sp =>
        {
            var loggerConfig =
                sp.GetRequiredService<ILoggerConfigurationBuilder>();

            return new PaymentsSerilogLoggerFactory(loggerConfig);
        });

        services.AddSingleton<IPaymentLogger, PaymentLogger>();

        return services;
    }
}