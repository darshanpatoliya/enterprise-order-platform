using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.Domain.Carts;

public sealed class Cart
{
    private readonly List<CartItem> _items = new();

    public Guid Id { get; }
    public Guid CustomerId { get; }

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public Cart(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID is required.",
                nameof(customerId));
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
    }

    public void AddItem(
        Guid productId,
        string productName,
        Money unitPrice,
        int quantity)
    {
        var existingItem = _items.FirstOrDefault(
            item => item.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        var newItem = new CartItem(
            productId,
            productName,
            unitPrice,
            quantity);

        _items.Add(newItem);
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(
            item => item.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "The product does not exist in the cart.");
        }

        _items.Remove(item);
    }

    public void ChangeQuantity(Guid productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        var item = _items.FirstOrDefault(
            item => item.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "The product does not exist in the cart.");
        }

        item.SetQuantity(quantity);
    }

    public Money GetTotal()
    {
        if (_items.Count == 0)
        {
            return Money.Zero("CAD");
        }

        var total = Money.Zero(
            _items.First().UnitPrice.Currency);

        foreach (var item in _items)
        {
            total = total.Add(item.GetLineTotal());
        }

        return total;
    }

    public void Clear()
    {
        _items.Clear();
    }
}