using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.MonthlySummaries;

internal static class MonthlySummaryTestHelpers
{
    private const string DefaultPassword = "Testing1234!";

    /// <summary>Creates a Platform and an Active Rider Employee assigned to it. Most
    /// GenerateMonthlySummary tests need the platform id to attach a SalaryFormula to.</summary>
    public static async Task<(Guid EmployeeId, Guid PlatformId)> CreateRiderOnPlatformAsync()
    {
        var platform = Platform.Create($"Platform-{Guid.NewGuid():N}");
        await TestApp.AddAsync(platform);

        var unique = Guid.NewGuid().ToString("N");
        var userId = await TestApp.RunAsUserAsync($"{unique}@test.local", DefaultPassword, []);
        var employee = Employee.Create(Guid.Parse(userId), "Rider One");
        employee.SubmitProfileForReview(
            iqamaNumber: $"TEST-{unique[..16]}",
            platformIdNumber: null, idExpiryDate: null, iqamaExpiryDate: null,
            drivingLicenseExpiryDate: null, insuranceExpiryDate: null);
        employee.Approve();
        employee.AssignPlatform(platform.Id);
        await TestApp.AddAsync(employee);

        return (employee.Id, platform.Id);
    }

    /// <summary>Creates a daily order for the given employee/date already in the target status
    /// (Open/Closed/Approved/Rejected), going through the same domain transitions the real
    /// review workflow would.</summary>
    public static async Task<Guid> CreateOrderAsync(Guid employeeId, DateOnly date, int completedOrders, DailyOrderStatus status)
    {
        var order = DailyOrder.StartForDate(employeeId, date);
        order.UpdateCount(completedOrders);

        if (status is DailyOrderStatus.Closed or DailyOrderStatus.Approved or DailyOrderStatus.Rejected)
            order.Close();
        if (status is DailyOrderStatus.Approved)
            order.Approve(Guid.NewGuid());
        if (status is DailyOrderStatus.Rejected)
            order.Reject(Guid.NewGuid(), "Test rejection.");

        await TestApp.AddAsync(order);
        return order.Id;
    }

    public static Task AddFixedMonthlyFormulaAsync(Guid? platformId, decimal amount, DateOnly effectiveFrom)
        => TestApp.AddAsync(SalaryFormula.CreateFixedMonthly(platformId, amount, effectiveFrom, Guid.NewGuid()));

    public static async Task AddTieredPerOrderFormulaAsync(Guid? platformId, DateOnly effectiveFrom, decimal ratePerOrder)
    {
        var formula = SalaryFormula.CreateTiered(platformId, effectiveFrom, Guid.NewGuid());
        formula.AddTier(minOrders: 1, maxOrders: null, SalaryTierRateType.PerOrder, ratePerOrder);
        await TestApp.AddAsync(formula);
    }

    /// <summary>Creates a real Identity user with the given roles and makes them the "current"
    /// TestApp identity — no linked Employee row, since none of the MonthlySummary handlers look
    /// the acting user up as an Employee (only [Authorize(Roles=...)] and IUser.Id matter).</summary>
    public static async Task<Guid> RunAsAsync(params string[] roles)
    {
        var userId = await TestApp.RunAsUserAsync($"{Guid.NewGuid():N}@test.local", DefaultPassword, roles);
        return Guid.Parse(userId);
    }
}
