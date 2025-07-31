using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.UnitTests.Builders;
using NServiceBus;
using NSubstitute;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories.BasketRepositoryTests;

public class SetQuantities
{
    private readonly CatalogContext _catalogContext;
    private readonly EfRepository<Basket> _basketRepository;
    private readonly BasketBuilder _basketBuilder = new();
    private readonly IMessageSession _mockSession = Substitute.For<IMessageSession>();


    public SetQuantities()
    {
        var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(databaseName: "TestCatalog")
            .Options;
        _catalogContext = new CatalogContext(dbOptions);
        _basketRepository = new EfRepository<Basket>(_catalogContext);
    }

    [Fact]
    public async Task RemoveEmptyQuantities()
    {
        var basket = _basketBuilder.WithOneBasketItem();
        var basketService = new BasketService(_basketRepository, null, _mockSession);
        await _basketRepository.AddAsync(basket, TestContext.Current.CancellationToken);
        _catalogContext.SaveChanges();

        await basketService.SetQuantities(_basketBuilder.BasketId, new Dictionary<string, int>() { { _basketBuilder.BasketId.ToString(), 0 } });

        Assert.Empty(basket.Items);
    }
}
