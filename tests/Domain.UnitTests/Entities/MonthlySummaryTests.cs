using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Domain.UnitTests.Entities;

public class MonthlySummaryTests
{
    private static readonly Guid EmployeeId = Guid.NewGuid();

    private static MonthlySummary DraftSummary(decimal totalSalary = 2500m, decimal totalAdvances = 200m, decimal totalFines = 50m)
        => MonthlySummary.Create(EmployeeId, 2026, 7, totalCompletedOrders: 100, totalSalary, totalAdvances, totalFines);

    private static MonthlySummary VerifiedSummary()
    {
        var summary = DraftSummary();
        summary.Verify(Guid.NewGuid());
        return summary;
    }

    [Test]
    public void Create_SetsFieldsAndStartsAsDraft()
    {
        var summary = MonthlySummary.Create(EmployeeId, 2026, 7, totalCompletedOrders: 100, totalSalary: 2500m, totalAdvances: 200m, totalFines: 50m);

        summary.EmployeeId.ShouldBe(EmployeeId);
        summary.Year.ShouldBe(2026);
        summary.Month.ShouldBe(7);
        summary.TotalCompletedOrders.ShouldBe(100);
        summary.Status.ShouldBe(MonthlySummaryStatus.Draft);
    }

    [TestCase(2500, 200, 50, 2250)]
    [TestCase(2500, 0, 0, 2500)]
    // Nothing in the entity stops advances+fines from exceeding salary — characterizing that
    // NetSalaryPayable can genuinely go negative rather than assuming it's clamped somewhere.
    [TestCase(500, 400, 300, -200)]
    public void NetSalaryPayable_IsTotalSalaryMinusAdvancesMinusFines(decimal salary, decimal advances, decimal fines, decimal expectedNet)
    {
        var summary = DraftSummary(salary, advances, fines);

        summary.NetSalaryPayable.ShouldBe(expectedNet);
    }

    [Test]
    public void Create_Throws_WhenEmployeeIdIsEmpty()
    {
        Should.Throw<ArgumentException>(() => MonthlySummary.Create(Guid.Empty, 2026, 7, 100, 2500m, 0m, 0m));
    }

    [TestCase(0)]
    [TestCase(13)]
    public void Create_Throws_WhenMonthIsOutOfRange(int month)
    {
        Should.Throw<ArgumentException>(() => MonthlySummary.Create(EmployeeId, 2026, month, 100, 2500m, 0m, 0m));
    }

    [Test]
    public void Verify_TransitionsDraftToVerified_AndStampsVerifier()
    {
        var summary = DraftSummary();
        var verifier = Guid.NewGuid();

        summary.Verify(verifier);

        summary.Status.ShouldBe(MonthlySummaryStatus.Verified);
        summary.VerifiedBy.ShouldBe(verifier);
        summary.VerifiedAt.ShouldNotBeNull();
    }

    [Test]
    public void Verify_Throws_WhenAlreadyVerified()
    {
        var summary = VerifiedSummary();

        Should.Throw<InvalidOperationException>(() => summary.Verify(Guid.NewGuid()));
    }

    [Test]
    public void MarkAsPaid_TransitionsVerifiedToPaid_AndStampsPaymentInfo()
    {
        var summary = VerifiedSummary();
        var payer = Guid.NewGuid();

        summary.MarkAsPaid(payer, "BANK-REF-123");

        summary.Status.ShouldBe(MonthlySummaryStatus.Paid);
        summary.PaidBy.ShouldBe(payer);
        summary.PaidAt.ShouldNotBeNull();
        summary.PaymentReference.ShouldBe("BANK-REF-123");
    }

    [Test]
    public void MarkAsPaid_Throws_WhenStillDraft()
    {
        var summary = DraftSummary(); // never verified

        Should.Throw<InvalidOperationException>(() => summary.MarkAsPaid(Guid.NewGuid(), null));
    }
}
