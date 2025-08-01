using System.Security.Cryptography.Xml;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq")
                 .WithImage("datalust/seq:2025.1")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithEnvironment("ACCEPT_EULA", "Y");

var rabbitUser = builder.AddParameter("rabbitUser", "rabbitUser");
var rabbitPassword = builder.AddParameter("rabbitPassword", "rabbitPassword");
var transport = builder.AddRabbitMQ("transport", rabbitUser, rabbitPassword, 5672)
                      .WithManagementPlugin(port: 15672);

//Particular Platform
var ravenDB = builder.AddContainer("ServiceControl-RavenDB", "particular/servicecontrol-ravendb")
    .WithHttpEndpoint(8080, 8080)
    .WithUrlForEndpoint("http", url => url.DisplayText = "Management Studio");

var audit = builder.AddContainer("ServiceControl-Audit", "particular/servicecontrol-audit")
    .WithEnvironment("TRANSPORTTYPE", "RabbitMQ.QuorumConventionalRouting")
    .WithEnvironment("CONNECTIONSTRING", transport)
    .WithEnvironment("RAVENDB_CONNECTIONSTRING", ravenDB.GetEndpoint("http"))
    .WithArgs("--setup-and-run")
    .WithHttpEndpoint(44444, 44444)
    .WithUrlForEndpoint("http", url => url.DisplayLocation = UrlDisplayLocation.DetailsOnly)
    .WithHttpHealthCheck("api/configuration")
    .WaitFor(transport)
    .WaitFor(ravenDB);

var serviceControl = builder.AddContainer("ServiceControl", "particular/servicecontrol")
    .WithEnvironment("TRANSPORTTYPE", "RabbitMQ.QuorumConventionalRouting")
    .WithEnvironment("CONNECTIONSTRING", transport)
    .WithEnvironment("RAVENDB_CONNECTIONSTRING", ravenDB.GetEndpoint("http"))
    .WithEnvironment("REMOTEINSTANCES", $"[{{\"api_uri\":\"{audit.GetEndpoint("http")}\"}}]")
    .WithArgs("--setup-and-run")
    .WithHttpEndpoint(33333, 33333)
    .WithUrlForEndpoint("http", url => url.DisplayLocation = UrlDisplayLocation.DetailsOnly)
    .WithHttpHealthCheck("api/configuration")
    .WaitFor(transport)
    .WaitFor(ravenDB);

var monitoring = builder.AddContainer("ServiceControl-Monitoring", "particular/servicecontrol-monitoring")
    .WithEnvironment("TRANSPORTTYPE", "RabbitMQ.QuorumConventionalRouting")
    .WithEnvironment("CONNECTIONSTRING", transport)
    .WithArgs("--setup-and-run")
    .WithHttpEndpoint(33633, 33633)
    .WithUrlForEndpoint("http", url => url.DisplayLocation = UrlDisplayLocation.DetailsOnly)
    .WithHttpHealthCheck("connection")
    .WaitFor(transport);

var servicePulse = builder.AddContainer("ServicePulse", "particular/servicepulse")
    .WithEnvironment("ENABLE_REVERSE_PROXY", "false")
    .WithHttpEndpoint(9090, 9090)
    .WithUrlForEndpoint("http", url => url.DisplayText = "ServicePulse")
    .WaitFor(serviceControl)
    .WaitFor(audit)
    .WaitFor(monitoring);

// Tie apps to Particular Platform
builder
    .AddProject<Projects.PublicApi>(nameof(Projects.PublicApi).ToLower())
        .WithReference(seq)
        .WaitFor(seq)
        .WaitFor(servicePulse);

builder.AddProject<Projects.eShopOnWeb_Worker>("NServiceBus-orders")
    .WaitFor(transport)
    .WaitFor(servicePulse);

builder
    .AddProject<Projects.Web>(nameof(Projects.Web).ToLower())
        .WithReference(seq)
        .WaitFor(seq)
        .WaitFor(transport)
        .WaitFor(servicePulse);

builder.AddProject<Projects.eShopOnNServiceBus_BasketsNServiceBusEndpoint>("NServiceBus-baskets")
    .WaitFor(transport)
    .WaitFor(servicePulse)
    .WaitFor(transport);

#pragma warning disable
builder.AddPythonApp("python-queue-listener", "../../", "./python/with-nservicebus-demo/python-rabbitmq-combined.py")
    .WaitFor(transport);

builder.Build().Run();
