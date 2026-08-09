namespace NerjaLogisticsERP.Domain.Entities;

public class DailyOrder : BaseAuditableEntity
{
    private DailyOrder() { }
    private DailyOrder(Guid employeeId, DateOnly orderDate)
    {
        EmployeeId = employeeId;
        OrderDate = orderDate;
    }

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    public DateOnly OrderDate { get; private set; }
    public int CompletedOrders { get; private set; }
    public DailyOrderStatus Status { get; private set; } = DailyOrderStatus.Open;
    public DateTimeOffset? ClosedAt { get; private set; }

    public static DailyOrder StartForDate(Guid employeeId, DateOnly orderDate)
    {
        if (employeeId == Guid.Empty)
            throw new ArgumentException("EmployeeId is required.", nameof(employeeId));

        var order = new DailyOrder(employeeId, orderDate);
        order.AddDomainEvent(new DailyOrderStartedEvent(order));
        return order;
    }

    public void UpdateCount(int completedOrders)
    {
        if (Status == DailyOrderStatus.Closed)
            throw new InvalidOperationException("Cannot update a closed daily order.");
        if (completedOrders < 0)
            throw new ArgumentException("Completed orders cannot be negative.", nameof(completedOrders));

        CompletedOrders = completedOrders;
    }

    public void Close()
    {
        if (Status == DailyOrderStatus.Closed)
            throw new InvalidOperationException("Daily order is already closed.");

        Status = DailyOrderStatus.Closed;
        ClosedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new DailyOrderClosedEvent(this));
    }
}
