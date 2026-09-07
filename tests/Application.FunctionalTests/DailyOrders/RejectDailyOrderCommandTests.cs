using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.DailyOrders.Commands.RejectDailyOrder;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.DailyOrders;

public class RejectDailyOrderCommandTests : TestBase
{
    [Test]
    public async Task ShouldReject_WhenSupervisorOwnsTheReport()
    {
        var (orderId, _, supervisorUserId, _) = await DailyOrderTestHelpers.CreateClosedOrderWithSupervisorAsync();

        await TestApp.SendAsync(new RejectDailyOrderCommand { DailyOrderId = orderId, Reason = "Count looks too high." });

        var order = await TestApp.FindAsync<DailyOrder>(orderId);
        order!.Status.ShouldBe(DailyOrderStatus.Rejected);
        order.ReviewedBy.ShouldBe(supervisorUserId);
        order.ReviewNote.ShouldBe("Count looks too high.");
    }

    [Test]
    public async Task ShouldThrowForbidden_WhenSupervisorDoesNotOwnTheReport()
    {
        var (orderId, _, _, _) = await DailyOrderTestHelpers.CreateClosedOrderWithSupervisorAsync();

        var (_, otherSupervisorUserId, otherSupervisorRoles) =
            await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Supervisor Two", Roles.Supervisor);
        TestApp.RunAs(otherSupervisorUserId, otherSupervisorRoles);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new RejectDailyOrderCommand { DailyOrderId = orderId, Reason = "Not my rider." }));
    }
}
