using NServiceBus;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate.Commands;
public class BasketToOrderConversionIncompleteCommand : ICommand
{
    public int BasketId { get; init; }
}
