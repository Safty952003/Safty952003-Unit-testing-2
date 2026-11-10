using FluentAssertions;
using global::Simulation.Checkout.Application;
using global::Simulation.Checkout.Application.Abstractions;
using Moq;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace Simulation.Checkout.Tests.TestData;




public class TestDataTests
{
    [Fact]
    public async Task CheckoutAsync_WhenOrderIsCreatedByAutoFixture_ShouldSucceed()
    {
        // Arrange
        var order = OrderFactory.Create();

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
        var result = await sut.CheckoutAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(400m);
    }


    [Fact]
    public async Task CheckoutAsync_WhenCustomerIsGeneratedByBogus_ShouldReturnSameCustomerDetails()
    {
        // Arrange
        var customer = CustomerFaker.Create();

        var order = new Order
        {
            CustomerName = customer.Name,
            CustomerEmail = customer.Email,
            Items = new List<OrderItem>
        {
            new()
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                UnitPrice = 100m,
                Quantity = 1
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
        var result = await sut.CheckoutAsync(order);

        // Assert
        result.CustomerName.Should().Be(customer.Name);
        result.CustomerEmail.Should().Be(customer.Email);
    }

}
