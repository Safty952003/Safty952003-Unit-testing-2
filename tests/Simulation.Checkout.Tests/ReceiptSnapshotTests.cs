
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using VerifyTests;
using VerifyXunit;
using Xunit;
namespace Simulation.Checkout.Tests;

public class ReceiptSnapshotTests
{
    [Fact]
    public async Task CheckoutAsync_ExampleC_ShouldMatchSnapshot()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CustomerName = "John Smith",
            CustomerEmail = "john.smith@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ProductName = "Test Product",
                    UnitPrice = 500m,
                    Quantity = 3
                }
            }
        };

        var orderRepository = new Mock<IOrderRepository>();
        var inventoryService = new Mock<IInventoryService>();
        var notificationService = new Mock<INotificationService>();

        inventoryService
            .Setup(x => x.IsAvailableAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new CheckoutService(
            orderRepository.Object,
            inventoryService.Object,
            notificationService.Object);

        // Act
        var result = sut.CheckoutAsync(order);

        // Assert
        var settings = new VerifySettings();
        settings.IgnoreMember("CreatedAt");

        await Verify(result, settings);
    }
}
