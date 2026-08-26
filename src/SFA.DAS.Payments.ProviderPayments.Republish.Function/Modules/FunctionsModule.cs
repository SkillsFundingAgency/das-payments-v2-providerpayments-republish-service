using Autofac;
using ESFA.DC.Logging.Interfaces;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Services;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Modules
{
    public class FunctionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EndpointInstanceFactory>()
                .As<IEndpointInstanceFactory>()
                .SingleInstance();

            builder.RegisterType<BlobStorageService>()
                .As<IBlobStorageService>()
                .SingleInstance();

            builder.RegisterType<ServiceBusMessageDeserializationService>()
                .As<IServiceBusMessageDeserializationService>()
                .SingleInstance();

            builder.RegisterType<CommandPublisherService>()
                .As<ICommandPublisherService>()
                .SingleInstance();

            builder.RegisterType<VersionInfo>()
                .As<IVersionInfo>()
                .SingleInstance();

            builder.RegisterType<ESFA.DC.Logging.ExecutionContext>()
                .As<IExecutionContext>()
                .SingleInstance();

            builder.RegisterType<ExecutionContextFactory>()
                .As<IExecutionContextFactory>()
                .SingleInstance();
        }
    }
}