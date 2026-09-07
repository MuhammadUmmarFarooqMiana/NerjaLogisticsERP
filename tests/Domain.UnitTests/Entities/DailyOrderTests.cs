using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Domain.UnitTests.Entities;

public class DailyOrderTests
{
    private static readonly Guid EmployeeId = Guid.NewGuid();
    private static readonly DateOnly Today = new(2026, 1, 15);

    private static DailyOrder OpenOrder() => DailyOrder.StartForDate(EmployeeId, Today);

    private static DailyOrder ClosedOrder()
    {
        var order = OpenOrder();
        order.Close();
        return order;
    }

    private static DailyOrder RejectedOrder()
    {
        var order = ClosedOrder();
        order.Reject(Guid.NewGuid(), "Count looked wrong.");
        return order;
    }

    [Test]
    public void StartForDate_CreatesOrder_WithOpenStatus()
    {
        var order = DailyOrder.StartForDate(EmployeeId, Today);

        order.Status.ShouldBe(DailyOrderStatus.Open);
        order.EmployeeId.ShouldBe(EmployeeId);
        order.OrderDate.ShouldBe(Today);
        order.CompletedOrders.ShouldBe(0);
    }

    [Test]
    public void StartForDate_Throws_WhenEmployeeIdIsEmpty()
    {
        Should.Throw<ArgumentException>(() => DailyOrder.StartForDate(Guid.Empty, Today));
    }

    [Test]
    public void UpdateCount_SetsCompletedOrders_WhileOpen()
    {
        var order = OpenOrder();

        order.UpdateCount(25);

        order.CompletedOrders.ShouldBe(25);
    }

    [Test]
    public void UpdateCount_Throws_WhenOrderIsNotOpen()
    {
        var order = ClosedOrder();

        Should.Throw<InvalidOperationException>(() => order.UpdateCount(10));
    }

    [Test]
    public void UpdateCount_Throws_WhenCountIsNegative()
    {
        var order = OpenOrder();

        Should.Throw<ArgumentException>(() => order.UpdateCount(-1));
    }

    [Test]
    public void Close_TransitionsOpenToClosed_AndStampsClosedAt()
    {
        var order = OpenOrder();

        order.Close();

        order.Status.ShouldBe(DailyOrderStatus.Closed);
        order.ClosedAt.ShouldNotBeNull();
    }

    [Test]
    public void Close_Throws_WhenOrderIsNotOpen()
    {
        var order = ClosedOrder(); // already Closed once

        Should.Throw<InvalidOperationException>(() => order.Close());
    }

    [Test]
    public void Approve_TransitionsClosedToApproved_AndStampsReviewer()
    {
        var order = ClosedOrder();
        var reviewer = Guid.NewGuid();

        order.Approve(reviewer);

        order.Status.ShouldBe(DailyOrderStatus.Approved);
        order.ReviewedBy.ShouldBe(reviewer);
        order.ReviewedAt.ShouldNotBeNull();
    }

    [Test]
    public void Approve_Throws_WhenOrderIsNotClosed()
    {
        var order = OpenOrder(); // never closed

        Should.Throw<InvalidOperationException>(() => order.Approve(Guid.NewGuid()));
    }

    [Test]
    public void Reject_TransitionsClosedToRejected_AndStoresReason()
    {
        var order = ClosedOrder();
        var reviewer = Guid.NewGuid();

        order.Reject(reviewer, "Numbers don't match the platform report.");

        order.Status.ShouldBe(DailyOrderStatus.Rejected);
        order.ReviewedBy.ShouldBe(reviewer);
        order.ReviewNote.ShouldBe("Numbers don't match the platform report.");
    }

    [Test]
    public void Reject_Throws_WhenOrderIsNotClosed()
    {
        var order = OpenOrder();

        Should.Throw<InvalidOperationException>(() => order.Reject(Guid.NewGuid(), "reason"));
    }

    [Test]
    public void Reject_Throws_WhenReasonIsBlank()
    {
        var order = ClosedOrder();

        Should.Throw<ArgumentException>(() => order.Reject(Guid.NewGuid(), "   "));
    }

    [Test]
    public void Correct_TransitionsRejectedToApproved_UpdatingCountAndReason()
    {
        var order = RejectedOrder();
        var reviewer = Guid.NewGuid();

        order.Correct(reviewer, completedOrders: 30, reason: "Verified against the platform export.");

        order.Status.ShouldBe(DailyOrderStatus.Approved);
        order.CompletedOrders.ShouldBe(30);
        order.ReviewedBy.ShouldBe(reviewer);
        order.ReviewNote.ShouldBe("Verified against the platform export.");
    }

    // Correct is only reachable from Rejected — a Closed (never-reviewed) or already-Approved
    // order must go through Approve/Reject first, not straight to Correct.
    [TestCase(DailyOrderStatus.Open)]
    [TestCase(DailyOrderStatus.Closed)]
    [TestCase(DailyOrderStatus.Approved)]
    public void Correct_Throws_WhenOrderIsNotRejected(DailyOrderStatus status)
    {
        var order = status switch
        {
            DailyOrderStatus.Open => OpenOrder(),
            DailyOrderStatus.Closed => ClosedOrder(),
            DailyOrderStatus.Approved => ApprovedOrder(),
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

        Should.Throw<InvalidOperationException>(() => order.Correct(Guid.NewGuid(), 10, "reason"));

        static DailyOrder ApprovedOrder()
        {
            var order = ClosedOrder();
            order.Approve(Guid.NewGuid());
            return order;
        }
    }

    [Test]
    public void Correct_Throws_WhenReasonIsBlank()
    {
        var order = RejectedOrder();

        Should.Throw<ArgumentException>(() => order.Correct(Guid.NewGuid(), 10, ""));
    }
}
