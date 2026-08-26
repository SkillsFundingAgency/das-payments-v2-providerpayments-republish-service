using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Services;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                return new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            })
            .As<IConfiguration>()
            .SingleInstance();

            builder.RegisterType<AzureFunctionConfigurationHelper>()
            .As<IConfigurationHelper>()
            .SingleInstance();
        }
    }
}