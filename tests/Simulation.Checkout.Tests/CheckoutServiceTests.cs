using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Domain.Exceptions;
using Xunit;


namespace Simulation.Checkout.Tests;

public class CheckoutServiceTests
{
    [Fact]
    public async Task CheckoutAsync_WhenOrderIsEmpty_ShouldThrowEmptyOrderException()
    {
        // Arrange
        var orderRepository = new Mock<IOrderRepository>();
        var inventoryService = new Mock<IInventoryService>();
        var notificationService = new Mock<INotificationService>();

        var sut = new CheckoutService(
            orderRepository.Object,
            inventoryService.Object,
            notificationService.Object);

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>()
        };

        // Act
        Func<Task> act = async () => await sut.CheckoutAsync(order);

        // Assert
        await act.Should().ThrowAsync<EmptyOrderException>();
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CheckoutAsync_WhenQuantityIsInvalid_ShouldThrowInvalidQuantityException(int quantity)
    {
        // Arrange
        var orderRepository = new Mock<IOrderRepository>();
        var inventoryService = new Mock<IInventoryService>();
        var notificationService = new Mock<INotificationService>();

        var sut = new CheckoutService(
            orderRepository.Object,
            inventoryService.Object,
            notificationService.Object);

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Laptop",
                UnitPrice = 1000m,
                Quantity = quantity
            }
        }
        };

        // Act
        Func<Task> act = async () => await sut.CheckoutAsync(order);

        // Assert
        var exception = await act.Should()
            .ThrowAsync<InvalidQuantityException>();

        exception.Which.Message.Should().Contain("Laptop");
    }


    [Fact]
    public async Task CheckoutAsync_WhenPriceIsNegative_ShouldThrowInvalidPriceException()
    {
        // Arrange
        var orderRepository = new Mock<IOrderRepository>();
        var inventoryService = new Mock<IInventoryService>();
        var notificationService = new Mock<INotificationService>();

        var sut = new CheckoutService(
            orderRepository.Object,
            inventoryService.Object,
            notificationService.Object);

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Laptop",
                UnitPrice = -0.01m,
                Quantity = 1
            }
        }
        };

        // Act
        Func<Task> act = async () => await sut.CheckoutAsync(order);

        // Assert
        await act.Should().ThrowAsync<InvalidPriceException>();
    }


    [Fact]
    public async Task CheckoutAsync_WhenPriceIsZero_ShouldSucceed()
    {
        // Arrange
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

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Free Product",
                UnitPrice = 0m,
                Quantity = 1
            }
        }
        };

        // Act
        var result = await sut.CheckoutAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Subtotal.Should().Be(0m);
        result.Discount.Should().Be(0m);
        result.Total.Should().Be(0m);
    }

    [Theory]
    [InlineData(999.99, 0)]
    [InlineData(1000, 100)]
    [InlineData(1500, 100)]
    public async Task CheckoutAsync_WhenSubtotalChanges_ShouldApplyCorrectDiscount(
        decimal subtotal,
        decimal expectedDiscount)
    {
        // Arrange
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

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                UnitPrice = subtotal,
                Quantity = 1
            }
        }
        };

        // Act
        var result = await sut.CheckoutAsync(order);

        // Assert
        result.Subtotal.Should().Be(subtotal);
        result.Discount.Should().Be(expectedDiscount);
    }

    
[Theory]
[InlineData(2, 150, 1, 400, 700, 0, 700)]
[InlineData(3, 500, 0, 0, 1500, 100, 1400)]
public async Task CheckoutAsync_WhenOrderHasMultipleItems_ShouldCalculateCorrectTotals(
    int firstQuantity,
    decimal firstPrice,
    int secondQuantity,
    decimal secondPrice,
    decimal expectedSubtotal,
    decimal expectedDiscount,
    decimal expectedTotal)
{
    // Arrange
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

        var items = new List<OrderItem>
{
    new OrderItem
    {
        ProductId = Guid.NewGuid(),
        ProductName = "Product A",
        UnitPrice = firstPrice,
        Quantity = firstQuantity
    }
};

        if (secondQuantity > 0)
        {
            items.Add(new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Product B",
                UnitPrice = secondPrice,
                Quantity = secondQuantity
            });
        }

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = items
        };

        // Act
        var result = await sut.CheckoutAsync(order);

    // Assert
    result.Should().NotBeNull();
    result.Subtotal.Should().Be(expectedSubtotal);
    result.Discount.Should().Be(expectedDiscount);
    result.Total.Should().Be(expectedTotal);
}



    [Fact]
    public async Task CheckoutAsync_WhenAllItemsAreInStock_ShouldSaveAndSendConfirmationOnce()
    {
        // Arrange
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

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Product A",
                UnitPrice = 150m,
                Quantity = 2
            },
            new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Product B",
                UnitPrice = 400m,
                Quantity = 1
            }
        }
        };

        // Act
        var result = await sut.CheckoutAsync(order);

        // Assert
        result.Should().NotBeNull();

         orderRepository
            .Verify(x => x.SaveAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

         notificationService
            .Verify(x => x.SendOrderConfirmationAsync(
                It.IsAny<CheckoutReceipt>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }



    [Fact]
    public async Task CheckoutAsync_WhenOneItemIsOutOfStock_ShouldThrowExceptionAndNotSaveOrNotify()
    {
        // Arrange
        var orderRepository = new Mock<IOrderRepository>();
        var inventoryService = new Mock<IInventoryService>();
        var notificationService = new Mock<INotificationService>();

        var availableProductId = Guid.NewGuid();
        var unavailableProductId = Guid.NewGuid();

        inventoryService
            .Setup(x => x.IsAvailableAsync(
                availableProductId,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        inventoryService
            .Setup(x => x.IsAvailableAsync(
                unavailableProductId,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new CheckoutService(
            orderRepository.Object,
            inventoryService.Object,
            notificationService.Object);

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = availableProductId,
                ProductName = "Product A",
                UnitPrice = 150m,
                Quantity = 2
            },
            new OrderItem
            {
                ProductId = unavailableProductId,
                ProductName = "Product B",
                UnitPrice = 400m,
                Quantity = 1
            }
        }
        };

        // Act
        var act = async () => await sut.CheckoutAsync(order);

        // Assert
        await act.Should()
            .ThrowAsync<InsufficientInventoryException>();

        orderRepository.Verify(
            x => x.SaveAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        notificationService.Verify(
            x => x.SendOrderConfirmationAsync(
                It.IsAny<CheckoutReceipt>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }



    [Fact]
    public async Task CheckoutAsync_WhenQuantityIsZero_ShouldThrowExceptionWithoutCallingServices()
    {
        // Arrange
        var orderRepository = new Mock<IOrderRepository>();
        var inventoryService = new Mock<IInventoryService>();
        var notificationService = new Mock<INotificationService>();

        var sut = new CheckoutService(
            orderRepository.Object,
            inventoryService.Object,
            notificationService.Object);

        var order = new Order
        {
            CustomerName = "Ahmed",
            CustomerEmail = "ahmed@test.com",
            Items = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Product A",
                UnitPrice = 150m,
                Quantity = 0
            }
        }
        };

        // Act
        var act = async () => await sut.CheckoutAsync(order);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidQuantityException>();

        inventoryService.Verify(
            x => x.IsAvailableAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        orderRepository.Verify(
            x => x.SaveAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        notificationService.Verify(
            x => x.SendOrderConfirmationAsync(
                It.IsAny<CheckoutReceipt>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }



    

}
