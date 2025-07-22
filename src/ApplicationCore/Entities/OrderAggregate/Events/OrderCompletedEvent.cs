using System;
using NServiceBus;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
public class OrderCompletedEvent : IMessage
{
    public int Id { get; init; }
    public string? BuyerId { get; init; }
    public int BasketId { get; init; }
    public DateTime OrderCompletionDate { get; init; } 
}
