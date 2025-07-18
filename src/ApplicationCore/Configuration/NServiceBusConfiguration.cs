using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
using NServiceBus;

namespace Microsoft.eShopWeb.ApplicationCore.Configuration;
public static class NServiceBusConfiguration
{
    public static EndpointConfiguration GetNServiceBusConfiguration()
    {
        var endpointConfiguration = new EndpointConfiguration("orders-worker");

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        // This is DEMO code
        // Normally, I wouldn't recommend disabling the remote certificate validation
        // However, we're running locally
        // Also, the connection string could be tied in via DI
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString("amqp://rabbitUser:rabbitPassword@localhost:5672");
        // We're using Direct routing to run on a single exchange
        transport.UseDirectRoutingTopology(QueueType.Quorum);
        transport.DisableRemoteCertificateValidation();
        
        transport.Routing().RouteToEndpoint(
          typeof(OrderCreatedEvent),
          "orders-worker");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");

        return endpointConfiguration;
    }
}
