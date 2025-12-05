using System;
using System.Collections.Generic;
using System.IO;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Services;

[assembly: WebJobsStartup(typeof(SFA.DAS.Payments.ProviderPayments.Republish.Function.Startup))]
namespace SFA.DAS.Payments.ProviderPayments.Republish.Function
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            configBuilder.AddJsonFile("local.settings.json", optional: true);

            var config = configBuilder.Build();
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));

            builder.Services.AddSingleton<IConfigurationHelper, AzureFunctionConfigurationHelper>();
            builder.Services.AddSingleton<IEndpointInstanceFactory, EndpointInstanceFactory>();
            builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
            builder.Services.AddSingleton<IServiceBusMessageDeserializationService, ServiceBusMessageDeserializationService>();
            builder.Services.AddSingleton<ICommandPublisherService, CommandPublisherService>();
            builder.Services.AddSingleton<IVersionInfo, VersionInfo>();
            builder.Services.AddSingleton<ILoggerConfigurationBuilder, PaymentsLoggerConfigurationBuilder>();
            builder.Services.AddSingleton<IApplicationLoggerSettings>(c =>
                {
                    var versionInfo = c.GetService<IVersionInfo>();
                    var configHelper = c.GetService<IConfigurationHelper>();

                    if (!Enum.TryParse(configHelper.GetSettingOrDefault("LogLevel", "Information"), out LogLevel logLevel))
                    {
                        logLevel = LogLevel.Information;
                    }

                    return new ApplicationLoggerSettings
                    {
                        ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                        {
                            new ConsoleApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = logLevel
                            },
                        },
                        TaskKey = versionInfo.ServiceReleaseVersion
                    };
                });
            builder.Services.AddSingleton<IExecutionContext, ESFA.DC.Logging.ExecutionContext>();
            builder.Services.AddSingleton<IExecutionContextFactory, ExecutionContextFactory>();
            builder.Services.AddSingleton<TelemetryConfiguration>(c =>
                {
                    var configHelper = c.GetService<IConfigurationHelper>();
                    return new TelemetryConfiguration(configHelper.GetSetting("ApplicationInsightsInstrumentationKey"));
                });
            builder.Services.AddSingleton<ISerilogLoggerFactory>((c =>
            {
                var config = c.GetService<ILoggerConfigurationBuilder>();

                return new PaymentsSerilogLoggerFactory(config);
            }));
            builder.Services.AddSingleton<IPaymentLogger, PaymentLogger>();
                
        }
    }
}
