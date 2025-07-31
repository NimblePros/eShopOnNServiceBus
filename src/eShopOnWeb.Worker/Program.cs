using Microsoft.eShopWeb.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);
var endpointConfiguration = NServiceBusConfiguration.GetOrderEndpointConfiguration((builder.Configuration.GetConnectionString("transport")!));
 endpointConfiguration.AuditSagaStateChanges(
    serviceControlQueue: "Particular.ServiceControl");

builder.UseNServiceBus(endpointConfiguration);

var host = builder.Build();

host.Run();
