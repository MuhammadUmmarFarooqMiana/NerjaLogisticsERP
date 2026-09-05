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
    public Guid? ReviewedBy { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    // Doubles as the rejection reason and, after a supervisor correction, the
    // correction note — whichever review action happened most recently. Mirrors
    // LeaveRequest.RejectionReason but under a more general name since this one
    // is reused by Correct(), not just Reject().
    public string? ReviewNote { get; private set; }

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
        if (Status != DailyOrderStatus.Open)
            throw new InvalidOperationException($"Cannot update a daily order with status {Status}.");
        if (completedOrders < 0)
            throw new ArgumentException("Completed orders cannot be negative.", nameof(completedOrders));

        CompletedOrders = completedOrders;
    }

    public void Close()
    {
        if (Status != DailyOrderStatus.Open)
            throw new InvalidOperationException($"Cannot close a daily order with status {Status}.");

        Status = DailyOrderStatus.Closed;
        ClosedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new DailyOrderClosedEvent(this));
    }

    // --- Supervisor/Administrator review, once the rider has closed for the day ---

    public void Approve(Guid reviewedBy)
    {
        if (Status != DailyOrderStatus.Closed)
            throw new InvalidOperationException($"Cannot approve a daily order with status {Status}.");

        Status = DailyOrderStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new DailyOrderApprovedEvent(this));
    }

    public void Reject(Guid reviewedBy, string reason)
    {
        if (Status != DailyOrderStatus.Closed)
            throw new InvalidOperationException($"Cannot reject a daily order with status {Status}.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A rejection reason is required.", nameof(reason));

        Status = DailyOrderStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTimeOffset.UtcNow;
        ReviewNote = reason;
        AddDomainEvent(new DailyOrderRejectedEvent(this));
    }

    // Only reachable from Rejected — the supervisor is correcting a count they
    // already flagged as wrong. Fixing it and approving it happen in the same
    // step since the supervisor has now personally verified the real number;
    // there's no reason to route it back through a second approval.
    public void Correct(Guid reviewedBy, int completedOrders, string reason)
    {
        if (Status != DailyOrderStatus.Rejected)
            throw new InvalidOperationException($"Cannot correct a daily order with status {Status}.");
        if (completedOrders < 0)
            throw new ArgumentException("Completed orders cannot be negative.", nameof(completedOrders));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A reason is required.", nameof(reason));

        CompletedOrders = completedOrders;
        Status = DailyOrderStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTimeOffset.UtcNow;
        ReviewNote = reason;
        AddDomainEvent(new DailyOrderCorrectedEvent(this));
    }
}
