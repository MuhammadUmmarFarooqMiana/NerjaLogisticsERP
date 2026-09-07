using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.DailyOrders.Commands.ApproveDailyOrder;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.DailyOrders;

public class ApproveDailyOrderCommandTests : TestBase
{
    [Test]
    public async Task ShouldApprove_WhenSupervisorOwnsTheReport()
    {
        var (orderId, _, supervisorUserId, _) = await DailyOrderTestHelpers.CreateClosedOrderWithSupervisorAsync();
        // still "current" = the owning supervisor

        await TestApp.SendAsync(new ApproveDailyOrderCommand { DailyOrderId = orderId });

        var order = await TestApp.FindAsync<DailyOrder>(orderId);
        order!.Status.ShouldBe(DailyOrderStatus.Approved);
        order.ReviewedBy.ShouldBe(supervisorUserId);
    }

    // Administrator is unrestricted — approves even though they have no supervisory
    // relationship to the rider at all, unlike a plain Supervisor.
    [Test]
    public async Task ShouldApprove_WhenAdministrator_RegardlessOfSupervisorRelationship()
    {
        var (orderId, _, _, _) = await DailyOrderTestHelpers.CreateClosedOrderWithSupervisorAsync();

        var (_, adminUserId, adminRoles) = await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Admin One", Roles.Administrator);
        TestApp.RunAs(adminUserId, adminRoles);

        await TestApp.SendAsync(new ApproveDailyOrderCommand { DailyOrderId = orderId });

        (await TestApp.FindAsync<DailyOrder>(orderId))!.Status.ShouldBe(DailyOrderStatus.Approved);
    }

    [Test]
    public async Task ShouldThrowForbidden_WhenSupervisorDoesNotOwnTheReport()
    {
        var (orderId, _, _, _) = await DailyOrderTestHelpers.CreateClosedOrderWithSupervisorAsync();

        var (_, otherSupervisorUserId, otherSupervisorRoles) =
            await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Supervisor Two", Roles.Supervisor);
        TestApp.RunAs(otherSupervisorUserId, otherSupervisorRoles);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new ApproveDailyOrderCommand { DailyOrderId = orderId }));
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenOrderDoesNotExist()
    {
        await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Admin One", Roles.Administrator);

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new ApproveDailyOrderCommand { DailyOrderId = Guid.NewGuid() }));
    }
}
