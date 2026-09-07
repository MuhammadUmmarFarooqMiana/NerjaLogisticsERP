using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.DailyOrders;
using NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.DailyOrders;

public class UpsertDailyOrderCommandTests : TestBase
{
    [Test]
    public async Task ShouldCreateTodaysOpenOrder_OnFirstCall()
    {
        var (employeeId, _, _) = await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Rider One");

        var orderId = await TestApp.SendAsync(new UpsertDailyOrderCommand { CompletedOrders = 12 });

        var order = await TestApp.FindAsync<DailyOrder>(orderId);
        order.ShouldNotBeNull();
        order.EmployeeId.ShouldBe(employeeId);
        order.OrderDate.ShouldBe(DailyOrderClock.Today());
        order.CompletedOrders.ShouldBe(12);
        order.Status.ShouldBe(DailyOrderStatus.Open);
    }

    // Same rider, same day, called again: updates the existing row rather than creating a
    // second one — last write wins, it's a running count for the day, not additive.
    [Test]
    public async Task ShouldUpdateTheSameOrder_WhenCalledAgainTheSameDay()
    {
        await DailyOrderTestHelpers.CreateActiveEmployeeAsync("Rider One");

        var firstId = await TestApp.SendAsync(new UpsertDailyOrderCommand { CompletedOrders = 10 });
        var secondId = await TestApp.SendAsync(new UpsertDailyOrderCommand { CompletedOrders = 25 });

        secondId.ShouldBe(firstId);
        var order = await TestApp.FindAsync<DailyOrder>(firstId);
        order!.CompletedOrders.ShouldBe(25);
        (await TestApp.CountAsync<DailyOrder>()).ShouldBe(1);
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenCallerHasNoEmployeeRecord()
    {
        await TestApp.RunAsUserAsync("identity-only@test.local", "Testing1234!", []);

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new UpsertDailyOrderCommand { CompletedOrders = 1 }));
    }
}
