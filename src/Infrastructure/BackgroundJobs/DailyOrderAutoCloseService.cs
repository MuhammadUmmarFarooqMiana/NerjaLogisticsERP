using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.DailyOrders;
using NerjaLogisticsERP.Domain.Enums;
using NerjaLogisticsERP.Infrastructure.Data;

namespace NerjaLogisticsERP.Infrastructure.BackgroundJobs;

// Closes any DailyOrder still Open once its KSA-local day has ended. Runs
// once per KSA midnight rather than polling, so a rider who never clicks
// "Close" doesn't leave the order open indefinitely.
public class DailyOrderAutoCloseService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyOrderAutoCloseService> _logger;

    public DailyOrderAutoCloseService(IServiceScopeFactory scopeFactory, ILogger<DailyOrderAutoCloseService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(GetDelayUntilNextKsaMidnight(), stoppingToken);
                await CloseStaleOrdersAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private static TimeSpan GetDelayUntilNextKsaMidnight()
    {
        var nowKsa = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, DailyOrderClock.KsaZone);
        var nextMidnightKsa = nowKsa.Date.AddDays(1);
        var delay = nextMidnightKsa - nowKsa.DateTime;

        // A tiny buffer avoids firing a moment before midnight due to timer drift.
        return delay < TimeSpan.Zero ? TimeSpan.FromSeconds(5) : delay + TimeSpan.FromSeconds(5);
    }

    private async Task CloseStaleOrdersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var todayKsa = DailyOrderClock.Today();

        var staleOpenOrders = await context.DailyOrders
            .Where(o => o.Status == DailyOrderStatus.Open && o.OrderDate < todayKsa)
            .ToListAsync(cancellationToken);

        foreach (var order in staleOpenOrders)
            order.Close();

        if (staleOpenOrders.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Auto-closed {Count} stale daily orders", staleOpenOrders.Count);
        }
    }
}
