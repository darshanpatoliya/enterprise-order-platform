using EnterpriseOrderPlatform.Domain.Orders;
using EnterpriseOrderPlatform.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseOrderPlatform.UnitTests;

public class OrderTests
{

    [Fact]
    public void AddItem_WithNewProduct_AddsOrderItem()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        var productId = Guid.NewGuid();
        var unitPrice = new Money(50m, "CAD");

        // Act
        order.AddItem(
            productId,
            "Wireless Keyboard",
            unitPrice,
            2);

        // Assert
        var item = Assert.Single(order.Items);

        Assert.Equal(productId, item.ProductId);
        Assert.Equal("Wireless Keyboard", item.ProductName);
        Assert.Equal(unitPrice, item.UnitPrice);
        Assert.Equal(2, item.Quantity);
    }


    [Fact]
    public void AddItem_WithExistingProduct_IncreasesQuantity()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        var productId = Guid.NewGuid();
        var unitPrice = new Money(50m, "CAD");

        order.AddItem(
            productId,
            "Wireless Keyboard",
            unitPrice,
            2);

        // Act
        order.AddItem(
            productId,
            "Wireless Keyboard",
            unitPrice,
            3);

        // Assert
        var item = Assert.Single(order.Items);

        Assert.Equal(productId, item.ProductId);
        Assert.Equal(5, item.Quantity);
    }



    [Fact]
    public void AddItem_WithDifferentProduct_AddsAnotherOrderItem()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        var firstProductId = Guid.NewGuid();
        var secondProductId = Guid.NewGuid();

        order.AddItem(
            firstProductId,
            "Wireless Keyboard",
            new Money(50m, "CAD"),
            2);

        // Act
        order.AddItem(
            secondProductId,
            "Wireless Mouse",
            new Money(30m, "CAD"),
            1);

        // Assert
        Assert.Equal(2, order.Items.Count);

        Assert.Contains(
            order.Items,
            item => item.ProductId == firstProductId);

        Assert.Contains(
            order.Items,
            item => item.ProductId == secondProductId);
    }


    [Fact]
    public void GetTotal_WithMultipleItems_ReturnsSumOfLineTotals()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        order.AddItem(
            Guid.NewGuid(),
            "Wireless Keyboard",
            new Money(50m, "CAD"),
            2);

        order.AddItem(
            Guid.NewGuid(),
            "Wireless Mouse",
            new Money(30m, "CAD"),
            1);

        // Act
        var total = order.GetTotal();

        // Assert
        Assert.Equal(130m, total.Amount);
        Assert.Equal("CAD", total.Currency);
    }


    [Fact]
    public void GetTotal_WithNoItems_ReturnsZeroMoney()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        // Act
        var total = order.GetTotal();

        // Assert
        Assert.Equal(0m, total.Amount);
        Assert.Equal("CAD", total.Currency);
    }



    // Order Status Transition Tests
    [Fact]
    public void Confirm_WhenOrderIsPendingPayment_ChangesStatusToConfirmed()
    {
        // Arrange
        var order = CreateOrderWithItem();

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Confirm_WhenOrderIsNotPendingPayment_ThrowsException()
    {
        // Arrange
        var order = CreateOrderWithItem();
        order.Confirm();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void StartPreparing_WhenOrderIsConfirmed_ChangesStatusToPreparing()
    {
        // Arrange
        var order = CreateOrderWithItem();
        order.Confirm();

        // Act
        order.StartPreparing();

        // Assert
        Assert.Equal(OrderStatus.Preparing, order.Status);
    }
    [Fact]
    public void StartPreparing_WhenOrderIsNotConfirmed_ThrowsException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => order.StartPreparing());
    }


    [Fact]
    public void Ship_WhenOrderIsPreparing_ChangesStatusToShipped()
    {
        // Arrange
        var order = CreateOrderWithItem();
        order.Confirm();
        order.StartPreparing();

        // Act
        order.Ship();

        // Assert
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }
    [Fact]
    public void Ship_WhenOrderIsNotPreparing_ThrowsException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => order.Ship());
    }


    [Fact]
    public void Deliver_WhenOrderIsShipped_ChangesStatusToDelivered()
    {
        // Arrange
        var order = CreateOrderWithItem();
        order.Confirm();
        order.StartPreparing();
        order.Ship();

        // Act
        order.Deliver();

        // Assert
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }
    [Fact]
    public void Deliver_WhenOrderIsNotShipped_ThrowsException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => order.Deliver());
    }


    // Cancel Order Tests
    [Fact]
    public void Cancel_WhenOrderIsPendingPayment_ChangesStatusToCancelled()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }
    [Fact]
    public void Cancel_WhenOrderIsConfirmed_ChangesStatusToCancelled()
    {
        // Arrange
        var order = CreateOrderWithItem();
        order.Confirm();

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }
    [Fact]
    public void Cancel_WhenOrderIsPreparing_ChangesStatusToCancelled()
    {
        // Arrange
        var order = CreateOrderWithItem();
        order.Confirm();
        order.StartPreparing();

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }


    // Canceling an order that has already been shipped is not allowed, and should throw an exception.
    [Fact]
    public void Cancel_WhenOrderIsShipped_ThrowsException()
    {
        // Arrange
        var order = CreateOrderWithItem();


        order.Confirm();
        order.StartPreparing();
        order.Ship();

        // Act & Assert

        // Attempting to cancel a shipped order should throw an InvalidOperationException.
        Assert.Throws<InvalidOperationException>(
            () => order.Cancel());

        // The failed cancellation must not change the order status.
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    // Canceling an order that has already been delivered is not allowed, and should throw an exception.
    [Fact]
    public void Cancel_WhenOrderIsDelivered_ThrowsException()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Confirm();
        order.StartPreparing();
        order.Ship();
        order.Deliver();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => order.Cancel());

        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Confirm_WhenOrderIsCancelled_ThrowsException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        order.Cancel();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // Starting to prepare an order that has already been cancelled is not allowed, and should throw an exception.
    [Fact]
    public void StartPreparing_WhenOrderIsCancelled_ThrowsException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        order.Cancel();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => order.StartPreparing());

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // Shipping an order that has already been cancelled is not allowed, and should throw an exception.
    [Fact]
    public void Ship_WhenOrderIsCancelled_ThrowsException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        order.Cancel();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => order.Ship());

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Confirm_WhenOrderHasNoItems_ThrowsException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
        Assert.Equal(OrderStatus.PendingPayment, order.Status);
    }

    // Helper method to create an order with a single item for testing purposes.
    private static Order CreateOrderWithItem()
    {
        var order = new Order(Guid.NewGuid());

        order.AddItem(
            Guid.NewGuid(),
            "Test Product",
            new Money(10m, "CAD"),
            1);

        return order;
    }
}
