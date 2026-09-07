using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.FunctionalTests.PlatformReconciliation;

internal static class PlatformReconciliationTestHelpers
{
    private const string DefaultPassword = "Testing1234!";

    public static async Task<Guid> CreatePlatformAsync()
    {
        var platform = Platform.Create($"Platform-{Guid.NewGuid():N}");
        await TestApp.AddAsync(platform);
        return platform.Id;
    }

    /// <summary>Creates an Active Employee on the given platform, with the given PlatformIdNumber
    /// (or none, if null — Nerja's side of an unmatchable row) and a MonthlySummary for
    /// year/month carrying the given completed-orders/salary figures. Those two things are
    /// exactly what GenerateReconciliationReportCommandHandler compares against the uploaded
    /// sheet's rows.</summary>
    public static async Task<Guid> CreateEmployeeWithSummaryAsync(
        Guid platformId, string? platformIdNumber, int year, int month,
        int totalCompletedOrders, decimal totalSalary, decimal totalAdvances = 0m, decimal totalFines = 0m)
    {
        var unique = Guid.NewGuid().ToString("N");
        var userId = await TestApp.RunAsUserAsync($"{unique}@test.local", DefaultPassword, []);
        var employee = Employee.Create(Guid.Parse(userId), $"Rider {unique[..8]}");
        employee.SubmitProfileForReview(
            iqamaNumber: $"TEST-{unique[..16]}",
            platformIdNumber: platformIdNumber,
            idExpiryDate: null, iqamaExpiryDate: null, drivingLicenseExpiryDate: null, insuranceExpiryDate: null);
        employee.Approve();
        employee.AssignPlatform(platformId);
        await TestApp.AddAsync(employee);

        var summary = MonthlySummary.Create(employee.Id, year, month, totalCompletedOrders, totalSalary, totalAdvances, totalFines);
        await TestApp.AddAsync(summary);

        return employee.Id;
    }

    public static async Task<Guid> RunAsAsync(params string[] roles)
        => Guid.Parse(await TestApp.RunAsUserAsync($"{Guid.NewGuid():N}@test.local", DefaultPassword, roles));
}
