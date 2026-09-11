using EnterpriseOrderPlatform.Domain.Carts;
using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.UnitTests;

public class CartItemTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesCartItem()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var unitPrice = new Money(10m, "CAD");

        // Act
        var item = new CartItem(
            productId,
            productName,
            unitPrice,
            2);

        // Assert
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(productName, item.ProductName);
        Assert.Equal(unitPrice, item.UnitPrice);
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ThrowsException()
    {
        // Arrange
        var unitPrice = new Money(10m, "CAD");

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new CartItem(
                Guid.Empty,
                "Test Product",
                unitPrice,
                1));
    }

    [Fact]
    public void Constructor_WithEmptyProductName_ThrowsException()
    {
        // Arrange
        var unitPrice = new Money(10m, "CAD");

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new CartItem(
                Guid.NewGuid(),
                "",
                unitPrice,
                1));
    }

    [Fact]
    public void Constructor_WithNullUnitPrice_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new CartItem(
                Guid.NewGuid(),
                "Test Product",
                null!,
                1));
    }

    [Fact]
    public void Constructor_WithZeroQuantity_ThrowsException()
    {
        // Arrange
        var unitPrice = new Money(10m, "CAD");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CartItem(
                Guid.NewGuid(),
                "Test Product",
                unitPrice,
                0));
    }

    [Fact]
    public void Constructor_WithNegativeQuantity_ThrowsException()
    {
        // Arrange
        var unitPrice = new Money(10m, "CAD");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CartItem(
                Guid.NewGuid(),
                "Test Product",
                unitPrice,
                -1));
    }

    [Fact]
    public void IncreaseQuantity_WithValidQuantity_IncreasesQuantity()
    {
        // Arrange
        var item = CreateCartItem(quantity: 2);

        // Act
        item.IncreaseQuantity(3);

        // Assert
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void IncreaseQuantity_WithZeroQuantity_ThrowsException()
    {
        // Arrange
        var item = CreateCartItem(quantity: 2);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => item.IncreaseQuantity(0));
    }

    [Fact]
    public void IncreaseQuantity_WithNegativeQuantity_ThrowsException()
    {
        // Arrange
        var item = CreateCartItem(quantity: 2);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => item.IncreaseQuantity(-1));
    }

    [Fact]
    public void GetLineTotal_ReturnsUnitPriceMultipliedByQuantity()
    {
        // Arrange
        var item = CreateCartItem(
            unitPrice: new Money(15m, "CAD"),
            quantity: 3);

        // Act
        var total = item.GetLineTotal();

        // Assert
        Assert.Equal(new Money(45m, "CAD"), total);
    }

    private static CartItem CreateCartItem(
        Money? unitPrice = null,
        int quantity = 1)
    {
        return new CartItem(
            Guid.NewGuid(),
            "Test Product",
            unitPrice ?? new Money(10m, "CAD"),
            quantity);
    }
}