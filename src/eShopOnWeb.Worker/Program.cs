using System.Text.Json;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.eShopWeb.ApplicationCore.Configuration;
using NServiceBus;
using NServiceBus.Installation;

var builder = Host.CreateDefaultBuilder();

builder.UseConsoleLifetime();
builder.UseNServiceBus(context => NServiceBusConfiguration.GetNServiceBusConfiguration());
await Installer.Setup(NServiceBusConfiguration.GetNServiceBusConfiguration());

var host = builder.Build();

host.Run();
