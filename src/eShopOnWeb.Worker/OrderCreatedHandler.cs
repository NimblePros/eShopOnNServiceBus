using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;

namespace eShopOnWeb.Worker;

public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
: IHandleMessages<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger = logger;

    public async Task Handle(OrderCreatedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("{OrderDate} - Received {EventName} - Order {OrderId} from buyer {BuyerId}: {OrderTotal:C}\nTotal Units: {itemCount}\nTotal Line Items: {uniqueItems}",
          nameof(OrderCreatedEvent),
          message.OrderDate, message.Id, message.BuyerId, message.Total,
          message.Items.Sum(x => x.Units),
          message.Items.Count);
        // Adding time between creation and completion
        Random random = new Random();
        await Task.Delay(1000 * random.Next(1, 29));
        await context.Send("orders", new OrderCompletedEvent
        {
            Id = message.Id,
            OrderCompletionDate = DateTime.UtcNow,
            BasketId = message.BasketId,
            BuyerId = message.BuyerId
        }
    );
        return;
    }
}

