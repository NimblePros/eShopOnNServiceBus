using Microsoft.eShopWeb.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.UseNServiceBus(NServiceBusConfiguration.GetBasketEndpointConfiguration((builder.Configuration.GetConnectionString("transport")!)));

var host = builder.Build();
host.Run();
