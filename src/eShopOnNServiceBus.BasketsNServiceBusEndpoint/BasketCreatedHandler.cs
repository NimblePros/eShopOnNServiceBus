using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Events;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;

namespace eShopOnNServiceBus.BasketsNServiceBusEndpoint;
public class BasketCreatedHandler(ILogger<BasketCreatedHandler> logger)
: IHandleMessages<BasketCreatedEvent>
{
    private readonly ILogger<BasketCreatedHandler> _logger = logger;

    public Task Handle(BasketCreatedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Basket {Id} Created for buyer {BuyerId} with {Count} items",
          message.Id, message.BuyerId, message.TotalItems);
        return Task.CompletedTask;
    }
}

