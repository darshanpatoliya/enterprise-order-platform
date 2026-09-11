using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; }
    public Guid CustomerId { get; }
    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID is required.",
                nameof(customerId));
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.PendingPayment;
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

        var newItem = new OrderItem(
            productId,
            productName,
            unitPrice,
            quantity);

        _items.Add(newItem);
    }


    public Money GetTotal()
    {
        var total = Money.Zero("CAD");

        foreach (var item in _items)
        {
            total = total.Add(item.GetLineTotal());
        }

        return total;
    }
}