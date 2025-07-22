using NServiceBus;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Commands;
public class StartBasketTrackingCommand : ICommand
{
    public int BasketId {  get; init; }
}
