using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;

namespace eShopOnWeb.Worker;
public class OrderCompletedHandler(ILogger<OrderCompletedHandler> logger)
  : IHandleMessages<OrderCompletedEvent>
    {
        private readonly ILogger<OrderCompletedHandler> _logger = logger;

    public Task Handle(OrderCompletedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Order Id: {Id}\nBuyer Id: {BuyerId}\nBasket Id: {BasketId}\nOrder Completion Date: {CompletionDate}",
          message.Id,
          message.BuyerId,
          message.BasketId,
          message.OrderCompletionDate);
        return Task.CompletedTask;
    }
}
