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
}