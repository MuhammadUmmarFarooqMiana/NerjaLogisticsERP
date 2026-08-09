namespace NerjaLogisticsERP.Domain.Entities;

public class LeaveRequest : BaseAuditableEntity
{
    private LeaveRequest() { }
    private LeaveRequest(Guid employeeId, DateOnly startDate, DateOnly endDate, string? reason)
    {
        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason;
    }

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string? Reason { get; private set; }
    public LeaveStatus Status { get; private set; } = LeaveStatus.Pending;
    public Guid? ReviewedBy { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public static LeaveRequest Submit(Guid employeeId, DateOnly startDate, DateOnly endDate, string? reason)
    {
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));
        if (endDate < startDate) throw new ArgumentException("EndDate cannot be before StartDate.", nameof(endDate));

        var leave = new LeaveRequest(employeeId, startDate, endDate, reason);
        leave.AddDomainEvent(new LeaveRequestSubmittedEvent(leave));
        return leave;
    }

    public void Approve(Guid reviewedBy)
    {
        if (Status != LeaveStatus.Pending)
            throw new InvalidOperationException($"Cannot approve a leave request with status {Status}.");

        Status = LeaveStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new LeaveRequestApprovedEvent(this));
    }

    public void Reject(Guid reviewedBy, string reason)
    {
        if (Status != LeaveStatus.Pending)
            throw new InvalidOperationException($"Cannot reject a leave request with status {Status}.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A rejection reason is required.", nameof(reason));

        Status = LeaveStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTimeOffset.UtcNow;
        RejectionReason = reason;
        AddDomainEvent(new LeaveRequestRejectedEvent(this));
    }
}
