using ESFA.DC.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Services;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.ServiceRegistrations;

public static class FunctionServiceRegistration
{
    public static IServiceCollection AddFunctionServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IEndpointInstanceFactory, EndpointInstanceFactory>();

        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        services.AddSingleton<IServiceBusMessageDeserializationService,
            ServiceBusMessageDeserializationService>();

        services.AddSingleton<ICommandPublisherService,
            CommandPublisherService>();

        services.AddSingleton<IVersionInfo, VersionInfo>();

        services.AddSingleton<IExecutionContext,
            ESFA.DC.Logging.ExecutionContext>();

        services.AddSingleton<IExecutionContextFactory,
            ExecutionContextFactory>();

        return services;
    }
}