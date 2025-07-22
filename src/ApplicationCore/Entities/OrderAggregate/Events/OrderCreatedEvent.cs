using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using NServiceBus;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
public class OrderCreatedEvent :IMessage
{
    public int Id { get; init; }
    public string? BuyerId { get; init; }
    public int BasketId {  get; init; }
    public DateTimeOffset OrderDate { get; init; }
    public decimal Total { get; init; }
    public IReadOnlyCollection<OrderItem> Items { get; init; } = [];
    public OrderCreatedEvent() { }

    public OrderCreatedEvent(int orderId, int basketId, string? buyerId, DateTimeOffset orderDate, decimal orderTotal, IReadOnlyCollection<OrderItem> orderItems) {
        Id = orderId;
        BasketId = basketId;
        BuyerId = buyerId;
        OrderDate = orderDate;
        Total = orderTotal;
        Items = orderItems;
    }
}
