using System;
using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Events;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
using NServiceBus;

namespace Microsoft.eShopWeb.Infrastructure.Configuration;
public static class NServiceBusConfiguration
{
    public static EndpointConfiguration GetNServiceBusConfiguration(string transportConnectionString, string queueName, List<Type> messageTypes)
    {
        var endpointConfiguration = new EndpointConfiguration(queueName);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(transportConnectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        // Only disabling in dev - DO NOT USE THIS IN PRODUCTION
        transport.DisableRemoteCertificateValidation();

        // Route the domain events
        foreach (var message in messageTypes) {
            transport.Routing().RouteToEndpoint(
              message,
              queueName); 
        };

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

    public static EndpointConfiguration GetOrderEndpointConfiguration(string transportConnectionString)
    {
        List<Type> domainEventTypes = [];
        domainEventTypes.Add(typeof(OrderCreatedEvent));
        return NServiceBusConfiguration.GetNServiceBusConfiguration(transportConnectionString, "orders", domainEventTypes);
    }

    public static EndpointConfiguration GetBasketEndpointConfiguration(string transportConnectionString)
    {
        List<Type> domainEventTypes = [];
        domainEventTypes.Add(typeof(BasketCreatedEvent));
        return NServiceBusConfiguration.GetNServiceBusConfiguration(transportConnectionString, "baskets", domainEventTypes);
    }

    public static EndpointConfiguration RegisterMultipleEndpointsForWeb(string transportConnectionString)
    {
        var endpointConfiguration = new EndpointConfiguration("web");
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(transportConnectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        // Only disabling in dev - DO NOT USE THIS IN PRODUCTION
        transport.DisableRemoteCertificateValidation();

        transport.Routing().RouteToEndpoint(typeof(OrderCreatedEvent), "orders");
        transport.Routing().RouteToEndpoint(typeof(BasketCreatedEvent), "baskets");

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
