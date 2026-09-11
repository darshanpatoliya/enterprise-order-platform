using EnterpriseOrderPlatform.Domain.Orders;
using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.UnitTests;

public class OrderItemTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesOrderItem()
    {
        // Arrange
        var productId = Guid.NewGuid();
        const string productName = "Wireless Keyboard";
        var unitPrice = new Money(50m, "CAD");
        const int quantity = 2;

        // Act
        var orderItem = new OrderItem(
            productId,
            productName,
            unitPrice,
            quantity);

        // Assert
        Assert.NotEqual(Guid.Empty, orderItem.Id);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(productName, orderItem.ProductName);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
        Assert.Equal(quantity, orderItem.Quantity);
    }

    
    [Fact]
    public void Constructor_WithEmptyProductId_ThrowsArgumentException()
    {
        // Arrange
        var productId = Guid.Empty;
        const string productName = "Wireless Keyboard";
        var unitPrice = new Money(50m, "CAD");
        const int quantity = 2;

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new OrderItem(
                productId,
                productName,
                unitPrice,
                quantity));
    }

    [Fact]
    public void Constructor_WithEmptyProductName_ThrowsArgumentException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        const string productName = "";
        var unitPrice = new Money(50m, "CAD");
        const int quantity = 2;

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new OrderItem(
                productId,
                productName,
                unitPrice,
                quantity));
    }


    [Fact]
    public void Constructor_WithNullUnitPrice_ThrowsArgumentNullException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        const string productName = "Wireless Keyboard";
        const int quantity = 2;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new OrderItem(
                productId,
                productName,
                null!,
                quantity));
    }


    [Fact]
    public void Constructor_WithZeroQuantity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        const string productName = "Wireless Keyboard";
        var unitPrice = new Money(50m, "CAD");
        const int quantity = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new OrderItem(
                productId,
                productName,
                unitPrice,
                quantity));
    }
    [Fact]
    public void Constructor_WithNegativeQuantity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        const string productName = "Wireless Keyboard";
        var unitPrice = new Money(50m, "CAD");
        const int quantity = -1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new OrderItem(
                productId,
                productName,
                unitPrice,
                quantity));
    }


    [Fact]
    public void GetLineTotal_ReturnsUnitPriceMultipliedByQuantity()
    {
        // Arrange
        var productId = Guid.NewGuid();
        const string productName = "Wireless Keyboard";
        var unitPrice = new Money(50m, "CAD");
        const int quantity = 3;

        var orderItem = new OrderItem(
            productId,
            productName,
            unitPrice,
            quantity);

        // Act
        var result = orderItem.GetLineTotal();

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal("CAD", result.Currency);
    }

    [Fact]
    public void IncreaseQuantity_WithValidQuantity_IncreasesQuantity()
    {
        // Arrange
        var orderItem = new OrderItem(
            Guid.NewGuid(),
            "Wireless Keyboard",
            new Money(50m, "CAD"),
            2);

        // Act
        orderItem.IncreaseQuantity(3);

        // Assert
        Assert.Equal(5, orderItem.Quantity);
    }

    [Fact]
    public void IncreaseQuantity_WithZeroQuantity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var orderItem = new OrderItem(
            Guid.NewGuid(),
            "Wireless Keyboard",
            new Money(50m, "CAD"),
            2);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => orderItem.IncreaseQuantity(0));
    }
    [Fact]
    public void IncreaseQuantity_WithNegativeQuantity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var orderItem = new OrderItem(
            Guid.NewGuid(),
            "Wireless Keyboard",
            new Money(50m, "CAD"),
            2);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>( 
            () => orderItem.IncreaseQuantity(-1));
    }
}