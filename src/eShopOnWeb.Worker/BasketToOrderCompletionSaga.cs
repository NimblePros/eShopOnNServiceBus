using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Commands;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;

namespace eShopOnWeb.Worker;
public class BasketToOrderCompletionSaga : Saga <BasketToOrderCompletionSagaData>,
    IAmStartedByMessages<StartBasketTrackingCommand>,
    IHandleMessages<StartBasketTrackingCommand>,
    IHandleMessages<OrderCreatedEvent>,
    IHandleMessages<OrderCompletedEvent>,
    IHandleMessages<BasketToOrderConversionIncompleteCommand>,
    IHandleTimeouts<BasketToOrderConversionSagaTimeout>,
    IHandleTimeouts<OrderCreationToOrderCompletionSagaTimeout>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BasketToOrderCompletionSagaData> mapper)
    {
        mapper.MapSaga(data => data.BasketId)
          .ToMessage<StartBasketTrackingCommand>(message => message.BasketId)
          .ToMessage<OrderCreatedEvent>(message => message.BasketId)
          .ToMessage<BasketToOrderConversionIncompleteCommand>(message => message.BasketId)
          .ToMessage<OrderCompletedEvent>(message => message.BasketId);
    }
    public async Task Handle(StartBasketTrackingCommand message, IMessageHandlerContext context)
    {
        // Timeout between basket and order creation
        var timeout = new BasketToOrderConversionSagaTimeout { BasketId = message.BasketId };
        await RequestTimeout(context, DateTime.UtcNow.AddSeconds(30), timeout);
    }

    public async Task Handle(OrderCreatedEvent message, IMessageHandlerContext context)
    {
        // Timeout between order creation and order completion
        var timeout = new OrderCreationToOrderCompletionSagaTimeout { BasketId = message.BasketId };
        await RequestTimeout(context, DateTime.UtcNow.AddSeconds(30), timeout);
    }



    public Task Handle(OrderCompletedEvent message, IMessageHandlerContext context)
    {
        MarkAsComplete();
        return Task.CompletedTask;
    }

    public Task Handle(BasketToOrderConversionIncompleteCommand message, IMessageHandlerContext context)
    {
        // TODO: Add logic to check on basket for abandonment
        return Task.CompletedTask;
    }
    public async Task Timeout(BasketToOrderConversionSagaTimeout state, IMessageHandlerContext context)
    {
        await context.Send("orders", new BasketToOrderConversionIncompleteCommand { BasketId = state.BasketId});
        MarkAsComplete();
    }

    public Task Timeout(OrderCreationToOrderCompletionSagaTimeout state, IMessageHandlerContext context)
    {
        // TODO: Add logic to check on order abandonment
        return Task.CompletedTask;
    }
}
public class BasketToOrderCompletionSagaData : ContainSagaData
{
    public int BasketId { get; set; }
}
