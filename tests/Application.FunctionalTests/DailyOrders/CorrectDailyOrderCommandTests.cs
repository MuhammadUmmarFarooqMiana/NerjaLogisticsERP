using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.DailyOrders.Commands.CorrectDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Commands.RejectDailyOrder;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.DailyOrders;

public class CorrectDailyOrderCommandTests : TestBase
{
    [Test]
    public async Task ShouldCorrectAndApprove_WhenOrderIsRejected()
    {
        var (orderId, _, _, _) = await DailyOrderTestHelpers.CreateClosedOrderWithSupervisorAsync(completedOrders: 10);
        await TestApp.SendAsync(new RejectDailyOrderCommand { DailyOrderId = orderId, Reason = "Wrong count." });
        // still "current" = the same supervisor who just rejected it

        await TestApp.SendAsync(new CorrectDailyOrderCommand
        {
            DailyOrderId = orderId,
            CompletedOrders = 18,
            Reason = "Verified against the platform export."
        });

        var order = await TestApp.FindAsync<DailyOrder>(orderId);
        order!.Status.ShouldBe(DailyOrderStatus.Approved);
        order.CompletedOrders.ShouldBe(18);
        order.ReviewNote.ShouldBe("Verified against the platform export.");
    }

    [Test]
    public async Task ShouldThrowForbidden_WhenSupervisorDoesNotOwnTheReport()
    {
        var (orderId, _, _, _) = await DailyOrderTestHelpers.CreateClosedOrderWithSupervisorAsync();
        await TestApp.SendAsync(new RejectDailyOrderCommand { DailyOrderId = orderId, Reason = "Wrong count." });

        var (_, otherSupervisorUserId, otherSupervisorRoles) =
            await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Supervisor Two", Roles.Supervisor);
        TestApp.RunAs(otherSupervisorUserId, otherSupervisorRoles);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new CorrectDailyOrderCommand { DailyOrderId = orderId, CompletedOrders = 5, Reason = "Not my rider." }));
    }
}
