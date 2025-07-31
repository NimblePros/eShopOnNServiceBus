using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Commands;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Events;

namespace eShopOnNServiceBus.BasketsNServiceBusEndpoint;
public class BasketCreatedHandler(ILogger<BasketCreatedHandler> logger)
: IHandleMessages<BasketCreatedEvent>
{
    private readonly ILogger<BasketCreatedHandler> _logger = logger;

    public async Task Handle(BasketCreatedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Basket {Id} Created for buyer {BuyerId} with {Count} items",
          message.Id, message.BuyerId, message.TotalItems);
        await context.Send(new StartBasketTrackingCommand
        {
            BasketId= message.Id,
        });
        return;
    }
}

