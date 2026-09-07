using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.DailyOrders.Commands.CloseMyDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.DailyOrders;

public class CloseMyDailyOrderCommandTests : TestBase
{
    [Test]
    public async Task ShouldCloseTodaysOpenOrder()
    {
        await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Rider One");
        var orderId = await TestApp.SendAsync(new UpsertDailyOrderCommand { CompletedOrders = 5 });

        await TestApp.SendAsync(new CloseMyDailyOrderCommand());

        var order = await TestApp.FindAsync<DailyOrder>(orderId);
        order!.Status.ShouldBe(DailyOrderStatus.Closed);
        order.ClosedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenNoOrderExistsForToday()
    {
        await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Rider One");
        // no UpsertDailyOrderCommand call — nothing to close

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new CloseMyDailyOrderCommand()));
    }
}
