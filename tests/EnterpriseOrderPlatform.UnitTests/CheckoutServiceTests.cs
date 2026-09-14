
using EnterpriseOrderPlatform.Application.Checkout;
using EnterpriseOrderPlatform.Domain.Carts;
using EnterpriseOrderPlatform.Domain.Orders;
using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.UnitTests;

public class CheckoutServiceTests
{
    [Fact]
    public void CreateOrderFromCart_CopiesCartItemIntoOrderItem()
    {
        // Arrange
        var cart = new Cart(Guid.NewGuid());
        var productId = Guid.NewGuid();

        cart.AddItem(
            productId,
            "Product A",
            new Money(25m, "CAD"),
            3);

        var service = new CheckoutService();

        // Act
        var order = service.CreateOrderFromCart(cart);

        // Assert
        var orderItem = Assert.Single(order.Items);

        Assert.Equal(OrderStatus.PendingPayment, order.Status);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal("Product A", orderItem.ProductName);
        Assert.Equal(new Money(25m, "CAD"), orderItem.UnitPrice);
        Assert.Equal(3, orderItem.Quantity);
    }

    [Fact]
    public void CreateOrderFromCart_WithMultipleCartItems_CopiesAllItems()
    {
        // Arrange
        var cart = new Cart(Guid.NewGuid());

        cart.AddItem(
            Guid.NewGuid(),
            "Product A",
            new Money(10m, "CAD"),
            2);

        cart.AddItem(
            Guid.NewGuid(),
            "Product B",
            new Money(15m, "CAD"),
            1);

        var service = new CheckoutService();

        // Act
        var order = service.CreateOrderFromCart(cart);

        // Assert
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(new Money(35m, "CAD"), order.GetTotal());
    }

    [Fact]
    public void CreateOrderFromCart_WithEmptyCart_ThrowsException()
    {
        // Arrange
        var cart = new Cart(Guid.NewGuid());
        var service = new CheckoutService();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => service.CreateOrderFromCart(cart));
    }

    [Fact]
    public void CreateOrderFromCart_DoesNotClearCart()
    {
        // Arrange
        var cart = new Cart(Guid.NewGuid());

        cart.AddItem(
            Guid.NewGuid(),
            "Product A",
            new Money(20m, "CAD"),
            2);

        var service = new CheckoutService();

        // Act
        service.CreateOrderFromCart(cart);

        // Assert
        Assert.Single(cart.Items);
    }

    
}
