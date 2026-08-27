using Microsoft.Extensions.Hosting;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.ServiceRegistrations;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services
            .AddConfigurationServices()
            .AddFunctionServices()
            .AddLoggingServices()
            .AddTelemetryServices();
    })
    .Build();

host.Run();