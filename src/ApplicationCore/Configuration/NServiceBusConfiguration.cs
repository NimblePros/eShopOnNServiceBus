using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
using NServiceBus;

namespace Microsoft.eShopWeb.ApplicationCore.Configuration;
public static class NServiceBusConfiguration
{
    public static EndpointConfiguration GetNServiceBusConfiguration(string transportConnectionString)
    {
        var endpointConfiguration = new EndpointConfiguration("orders");
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(transportConnectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        // Only disabling in dev - DO NOT USE THIS IN PRODUCTION
        transport.DisableRemoteCertificateValidation();
        
        transport.Routing().RouteToEndpoint(
          typeof(OrderCreatedEvent),
          "orders");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.EnableOpenTelemetry();
        endpointConfiguration.EnableInstallers();

        endpointConfiguration.SendHeartbeatTo(
            serviceControlQueue: "Particular.ServiceControl", //confirmed Queue name in RabbitMQ
            frequency: TimeSpan.FromSeconds(15),
            timeToLive: TimeSpan.FromSeconds(30));

        var metrics = endpointConfiguration.EnableMetrics();

        metrics.SendMetricDataToServiceControl(
            serviceControlMetricsAddress: "Particular.Monitoring", //confirmed Queue name in RabbitMQ
            interval: TimeSpan.FromMinutes(1));

        return endpointConfiguration;
    }
}
