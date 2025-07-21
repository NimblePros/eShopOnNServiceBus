using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using NServiceBus;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Events;
public class BasketCreatedEvent : IMessage
{
    public int Id { get; init; }
    public string? BuyerId { get; init; }
    public DateTimeOffset BasketCreatedDate { get; init; }
    private readonly List<BasketItem> _items = new List<BasketItem>();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public int TotalItems => _items.Sum(i => i.Quantity);
    public BasketCreatedEvent() { }
    public BasketCreatedEvent(int basketId, string? buyerId, DateTimeOffset basketCreatedDate, List<BasketItem> items)
    {
        Id = basketId;
        BuyerId = buyerId;
        BasketCreatedDate = basketCreatedDate;
        _items = items;
    }
}
