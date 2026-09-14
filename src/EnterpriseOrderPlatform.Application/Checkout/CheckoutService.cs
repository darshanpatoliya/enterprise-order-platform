using EnterpriseOrderPlatform.Domain.Carts;
using EnterpriseOrderPlatform.Domain.Orders;

namespace EnterpriseOrderPlatform.Application.Checkout;

public sealed class CheckoutService
{
    public Order CreateOrderFromCart(Cart cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot create an order from an empty cart.");
        }

        var order = new Order(cart.CustomerId);

        foreach (var cartItem in cart.Items)
        {
            order.AddItem(
                cartItem.ProductId,
                cartItem.ProductName,
                cartItem.UnitPrice,
                cartItem.Quantity);
        }

        return order;
    }
}