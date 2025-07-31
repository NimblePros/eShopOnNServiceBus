using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Commands;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Events;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
using NServiceBus;

namespace Microsoft.eShopWeb.Infrastructure.Configuration;
public static class NServiceBusConfiguration
{
    public static EndpointConfiguration GetNServiceBusConfiguration(string transportConnectionString, string queueName, Dictionary<Type,string> messageTypesToQueues)
    {
        var endpointConfiguration = new EndpointConfiguration(queueName);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(transportConnectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        // Only disabling in dev - DO NOT USE THIS IN PRODUCTION
        transport.DisableRemoteCertificateValidation();

        // Route the domain events
        foreach (var messageType in messageTypesToQueues.Keys) {
            transport.Routing().RouteToEndpoint(
              messageType,
              messageTypesToQueues[messageType]); 
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
        Dictionary<Type, string> messagingRouting = new();
        messagingRouting.Add(typeof(OrderCreatedEvent), "orders");
        var endpointConfig = NServiceBusConfiguration.GetNServiceBusConfiguration(transportConnectionString, "orders", messagingRouting);
        // ONLY USE THIS IN DEV - DO NOT USE IN PRODUCTION
        endpointConfig.UsePersistence<LearningPersistence>();
        return endpointConfig;
    }

    public static EndpointConfiguration GetBasketEndpointConfiguration(string transportConnectionString)
    {
        Dictionary<Type,string> messagingRouting = new();
        messagingRouting.Add(typeof(BasketCreatedEvent),"baskets");
        messagingRouting.Add(typeof(StartBasketTrackingCommand),"orders");
        return NServiceBusConfiguration.GetNServiceBusConfiguration(transportConnectionString, "baskets", messagingRouting);
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
