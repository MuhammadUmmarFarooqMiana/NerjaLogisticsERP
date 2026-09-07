using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.MarkMonthlySummaryAsPaid;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.VerifyMonthlySummary;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.MonthlySummaries;

public class MarkMonthlySummaryAsPaidCommandTests : TestBase
{
    private static readonly DateOnly FormulaEffectiveFrom = new(2026, 1, 1);

    private static async Task<Guid> CreateVerifiedSummaryAsync()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);
        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = 2026, Month = 7 });
        await TestApp.SendAsync(new VerifyMonthlySummaryCommand { Id = summaryId });
        return summaryId;
    }

    [Test]
    public async Task ShouldMarkAsPaid_WhenVerified_StoringThePaymentReference()
    {
        var summaryId = await CreateVerifiedSummaryAsync();

        await TestApp.SendAsync(new MarkMonthlySummaryAsPaidCommand { Id = summaryId, PaymentReference = "BANK-REF-123" });

        var summary = await TestApp.FindAsync<MonthlySummary>(summaryId);
        summary!.Status.ShouldBe(MonthlySummaryStatus.Paid);
        summary.PaymentReference.ShouldBe("BANK-REF-123");
        summary.PaidAt.ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenSummaryDoesNotExist()
    {
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new MarkMonthlySummaryAsPaidCommand { Id = Guid.NewGuid() }));
    }
}
