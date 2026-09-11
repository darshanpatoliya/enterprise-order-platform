namespace EnterpriseOrderPlatform.Domain.Orders;

public enum OrderStatus
{
    PendingPayment,
    Confirmed,
    Preparing,
    Shipped,
    Delivered,
    Cancelled
}