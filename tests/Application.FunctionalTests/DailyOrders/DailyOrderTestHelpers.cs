using NerjaLogisticsERP.Application.DailyOrders.Commands.CloseMyDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.FunctionalTests.DailyOrders;

internal static class DailyOrderTestHelpers
{
    private const string DefaultPassword = "Testing1234!";

    /// <summary>
    /// Creates a real Identity user with the given roles plus a linked, Active Employee — and
    /// leaves that user as the "current" TestApp identity (matching RunAsUserAsync's own
    /// behavior), so the immediate next SendAsync call acts as them. Returns everything a caller
    /// might need to switch back to this identity later via TestApp.RunAs.
    /// </summary>
    public static async Task<(Guid EmployeeId, Guid UserId, string[] Roles)> CreateActiveEmployeeAsync(string fullName, params string[] roles)
    {
        var unique = Guid.NewGuid().ToString("N");
        var email = $"{unique}@test.local";
        var userId = await TestApp.RunAsUserAsync(email, DefaultPassword, roles);

        var employee = Employee.Create(Guid.Parse(userId), fullName);
        employee.SubmitProfileForReview(
            // Employee.IqamaNumber has a real unique index — this helper can create more than
            // one employee per test (a rider plus their supervisor, or a second unrelated
            // supervisor), so a fixed value here would collide the moment a second one saves.
            iqamaNumber: $"TEST-{unique[..16]}",
            platformIdNumber: null,
            idExpiryDate: null,
            iqamaExpiryDate: null,
            drivingLicenseExpiryDate: null,
            insuranceExpiryDate: null);
        employee.Approve();

        await TestApp.AddAsync(employee);

        return (employee.Id, Guid.Parse(userId), roles);
    }

    /// <summary>
    /// Creates a Supervisor, a Rider linked to that supervisor, and the rider's daily order
    /// already submitted and Closed — the common starting point for every Approve/Reject/Correct
    /// test. Leaves the SUPERVISOR as the "current" identity when it returns, since that's what
    /// most callers need immediately (TestApp.RunAs to switch to an Administrator or an
    /// unrelated supervisor instead, for the tests that need one of those).
    /// </summary>
    public static async Task<(Guid OrderId, Guid SupervisorEmployeeId, Guid SupervisorUserId, string[] SupervisorRoles)>
        CreateClosedOrderWithSupervisorAsync(int completedOrders = 10)
    {
        var (supervisorEmployeeId, supervisorUserId, supervisorRoles) =
            await CreateActiveEmployeeAsync("Supervisor One", Roles.Supervisor);

        var (riderEmployeeId, _, _) = await CreateActiveEmployeeAsync("Rider One");

        var rider = await TestApp.FindAsync<Employee>(riderEmployeeId)
            ?? throw new InvalidOperationException("Seeded rider not found.");
        rider.AssignSupervisor(supervisorEmployeeId);
        await TestApp.UpdateAsync(rider);

        var orderId = await TestApp.SendAsync(new UpsertDailyOrderCommand { CompletedOrders = completedOrders });
        await TestApp.SendAsync(new CloseMyDailyOrderCommand());

        TestApp.RunAs(supervisorUserId, supervisorRoles);

        return (orderId, supervisorEmployeeId, supervisorUserId, supervisorRoles);
    }
}
