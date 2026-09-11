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
            throw new ArgumentException("Customer ID is required.", nameof(customerId));
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.PendingPayment;
    }


    public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
    {
        var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        var newItem = new OrderItem(productId, productName, unitPrice, quantity);

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

    // State transition methods
    public void Confirm()
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Only an order with pending payment can be confirmed.");
        }

        Status = OrderStatus.Confirmed;
    }
    public void StartPreparing()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("Only a confirmed order can start preparing.");
        }

        Status = OrderStatus.Preparing;
    }
    public void Ship()
    {
        if (Status != OrderStatus.Preparing)
        {
            throw new InvalidOperationException("Only an order that is being prepared can be shipped.");
        }

        Status = OrderStatus.Shipped;
    }
    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Only a shipped order can be delivered.");
        }

        Status = OrderStatus.Delivered;
    }
    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("A shipped or delivered order cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}