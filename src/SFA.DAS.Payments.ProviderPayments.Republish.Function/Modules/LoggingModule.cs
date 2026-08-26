using System;
using Autofac;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Modules
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PaymentsLoggerConfigurationBuilder>()
                .As<ILoggerConfigurationBuilder>()
                .SingleInstance();

            builder.Register(ctx =>
            {
                var versionInfo =
                    ctx.Resolve<IVersionInfo>();

                var configHelper =
                    ctx.Resolve<IConfigurationHelper>();

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
            })
            .As<IApplicationLoggerSettings>()
            .SingleInstance();

            builder.Register(ctx =>
            {
                var loggerConfig =
                    ctx.Resolve<ILoggerConfigurationBuilder>();

                return new PaymentsSerilogLoggerFactory(loggerConfig);
            })
            .As<ISerilogLoggerFactory>()
            .SingleInstance();

            builder.RegisterType<PaymentLogger>()
                .As<IPaymentLogger>()
                .SingleInstance();
        }
    }
}