using Autofac;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Modules
{
    public class TelemetryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                var configHelper =
                    ctx.Resolve<IConfigurationHelper>();

                return new TelemetryConfiguration(
                    configHelper.GetSetting(
                        "ApplicationInsightsInstrumentationKey"));
            })
            .SingleInstance();
        }
    }
}