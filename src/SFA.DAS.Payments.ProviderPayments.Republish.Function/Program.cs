using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Payments.ProviderPayments.Republish.Function.Modules;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<ConfigurationModule>();
        builder.RegisterModule<FunctionsModule>();
        builder.RegisterModule<LoggingModule>();
        builder.RegisterModule<TelemetryModule>();
    })
    .Build();

host.Run();