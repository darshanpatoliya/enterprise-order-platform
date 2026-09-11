using EnterpriseOrderPlatform.Domain.Carts;
using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.UnitTests;

public class CartTests
{
    [Fact]
    public void Constructor_WithValidCustomerId_CreatesCart()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var cart = new Cart(customerId);

        // Assert
        Assert.NotEqual(Guid.Empty, cart.Id);
        Assert.Equal(customerId, cart.CustomerId);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public void Constructor_WithEmptyCustomerId_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new Cart(Guid.Empty));
    }

    [Fact]
    public void AddItem_WithValidItem_AddsItemToCart()
    {
        // Arrange
        var cart = CreateCart();

        // Act
        cart.AddItem(
            Guid.NewGuid(),
            "Test Product",
            new Money(20m, "CAD"),
            2);

        // Assert
        var item = Assert.Single(cart.Items);

        Assert.Equal("Test Product", item.ProductName);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(new Money(20m, "CAD"), item.UnitPrice);
    }

    [Fact]
    public void AddItem_WithExistingProduct_IncreasesQuantity()
    {
        // Arrange
        var cart = CreateCart();
        var productId = Guid.NewGuid();

        cart.AddItem(
            productId,
            "Test Product",
            new Money(20m, "CAD"),
            2);

        // Act
        cart.AddItem(
            productId,
            "Test Product",
            new Money(20m, "CAD"),
            3);

        // Assert
        var item = Assert.Single(cart.Items);

        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void RemoveItem_WithExistingProduct_RemovesItem()
    {
        // Arrange
        var cart = CreateCart();
        var productId = Guid.NewGuid();

        cart.AddItem(
            productId,
            "Test Product",
            new Money(20m, "CAD"),
            1);

        // Act
        cart.RemoveItem(productId);

        // Assert
        Assert.Empty(cart.Items);
    }

    [Fact]
    public void RemoveItem_WithUnknownProduct_ThrowsException()
    {
        // Arrange
        var cart = CreateCart();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => cart.RemoveItem(Guid.NewGuid()));
    }

    [Fact]
    public void ChangeQuantity_WithExistingProduct_ChangesQuantity()
    {
        // Arrange
        var cart = CreateCart();
        var productId = Guid.NewGuid();

        cart.AddItem(
            productId,
            "Test Product",
            new Money(20m, "CAD"),
            2);

        // Act
        cart.ChangeQuantity(productId, 5);

        // Assert
        var item = Assert.Single(cart.Items);

        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void ChangeQuantity_WithZeroQuantity_ThrowsException()
    {
        // Arrange
        var cart = CreateCart();
        var productId = Guid.NewGuid();

        cart.AddItem(
            productId,
            "Test Product",
            new Money(20m, "CAD"),
            2);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => cart.ChangeQuantity(productId, 0));
    }

    [Fact]
    public void ChangeQuantity_WithUnknownProduct_ThrowsException()
    {
        // Arrange
        var cart = CreateCart();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => cart.ChangeQuantity(Guid.NewGuid(), 2));
    }

    [Fact]
    public void GetTotal_WithMultipleItems_ReturnsSumOfLineTotals()
    {
        // Arrange
        var cart = CreateCart();

        cart.AddItem(
            Guid.NewGuid(),
            "Product A",
            new Money(10m, "CAD"),
            2);

        cart.AddItem(
            Guid.NewGuid(),
            "Product B",
            new Money(15m, "CAD"),
            3);

        // Act
        var total = cart.GetTotal();

        // Assert
        Assert.Equal(new Money(65m, "CAD"), total);
    }

    [Fact]
    public void GetTotal_WithEmptyCart_ReturnsZeroMoney()
    {
        // Arrange
        var cart = CreateCart();

        // Act
        var total = cart.GetTotal();

        // Assert
        Assert.Equal(new Money(0m, "CAD"), total);
    }

    [Fact]
    public void Clear_WithItems_RemovesAllItems()
    {
        // Arrange
        var cart = CreateCart();

        cart.AddItem(
            Guid.NewGuid(),
            "Product A",
            new Money(10m, "CAD"),
            1);

        cart.AddItem(
            Guid.NewGuid(),
            "Product B",
            new Money(20m, "CAD"),
            2);

        // Act
        cart.Clear();

        // Assert
        Assert.Empty(cart.Items);
    }

    private static Cart CreateCart()
    {
        return new Cart(Guid.NewGuid());
    }
}