using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.Domain.Carts;

public sealed class CartItem
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public Money UnitPrice { get; }
    public int Quantity { get; private set; }

    public CartItem(
        Guid productId,
        string productName,
        Money unitPrice,
        int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product ID is required.",
                nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(productName));
        }

        ArgumentNullException.ThrowIfNull(unitPrice);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity to add must be greater than zero.");
        }

        Quantity += quantity;
    }

    public Money GetLineTotal()
    {
        return UnitPrice.Multiply(Quantity);
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        Quantity = quantity;
    }
}