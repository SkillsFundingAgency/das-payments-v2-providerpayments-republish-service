using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Services
{
    public interface ICommandPublisherService
    {
        Task<int> PublishCommandsToServiceBus(IEnumerable<ProcessProviderMonthEndAct1CompletionPaymentCommand> commands);
    }

    public class CommandPublisherService : ICommandPublisherService
    {
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IPaymentLogger _logger;
        
        public CommandPublisherService(IConfigurationHelper configurationHelper, IPaymentLogger logger)
        {
            _configurationHelper = configurationHelper;
            _logger = logger;
        }

        public async Task<int> PublishCommandsToServiceBus(IEnumerable<ProcessProviderMonthEndAct1CompletionPaymentCommand> commands)
        {
            var applicationConfig = ApplicationConfiguration.Create(_configurationHelper);
            var endpointConfig = EndpointConfigurationFactory.Create(applicationConfig);
            var scanner = endpointConfig.AssemblyScanner();
            scanner.ScanAssembliesInNestedDirectories = false;
            scanner.ScanFileSystemAssemblies = false;
            scanner.ExcludeAssemblies("*.*");
            endpointConfig.RegisterComponents(cfg => cfg.RegisterSingleton(_logger));
            var transport = endpointConfig.UseTransport<AzureServiceBusTransport>();
            transport.Routing().RouteToEndpoint(typeof(ProcessProviderMonthEndAct1CompletionPaymentCommand), _configurationHelper.GetSetting("EndpointName"));
            transport.ConnectionString(_configurationHelper.GetSetting("ServiceBusConnectionString"));

            var endpointInstance = await Endpoint.Start(endpointConfig);

            foreach (var command in commands)
            {
                await endpointInstance.Send(_configurationHelper.GetSetting("DestinationEndpointName"), command);
            }

            return commands.Count();
        }

        
    }
}
