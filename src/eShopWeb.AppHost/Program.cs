using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithEnvironment("ACCEPT_EULA", "Y");
builder
    .AddProject<Projects.PublicApi>(nameof(Projects.PublicApi).ToLower())
        .WithReference(seq)
        .WaitFor(seq);

var rabbitUser = builder.AddParameter("rabbitUser", "rabbitUser");
var rabbitPassword = builder.AddParameter("rabbitPassword", "rabbitPassword");
var rabbitmq = builder.AddRabbitMQ("messaging", rabbitUser, rabbitPassword, 5672)
                      .WithManagementPlugin(port: 15672);
builder.AddProject<Projects.eShopOnWeb_Worker>("eshoponweb-worker")
    .WaitFor(rabbitmq);

builder
    .AddProject<Projects.Web>(nameof(Projects.Web).ToLower())
        .WithReference(seq)
        .WaitFor(seq)
        .WaitFor(rabbitmq);

builder.Build().Run();
