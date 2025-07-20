using System.Text.Json;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.eShopWeb.ApplicationCore.Configuration;
using NServiceBus;
using NServiceBus.Installation;

var builder = Host.CreateApplicationBuilder(args);

builder.UseNServiceBus(NServiceBusConfiguration.GetNServiceBusConfiguration(builder.Configuration.GetConnectionString("transport")!));

var host = builder.Build();

host.Run();
