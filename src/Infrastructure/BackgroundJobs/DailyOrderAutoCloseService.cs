using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Domain.Enums;
using NerjaLogisticsERP.Infrastructure.Data;

namespace NerjaLogisticsERP.Infrastructure.BackgroundJobs;

public class DailyOrderAutoCloseService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyOrderAutoCloseService> _logger;
    private static readonly TimeZoneInfo KsaZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");

    public DailyOrderAutoCloseService(IServiceScopeFactory scopeFactory, ILogger<DailyOrderAutoCloseService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var todayKsa = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, KsaZone).Date);

            var staleOpenOrders = await context.DailyOrders
                .Where(o => o.Status == DailyOrderStatus.Open && o.OrderDate < todayKsa)
                .ToListAsync(stoppingToken);

            foreach (var order in staleOpenOrders)
                order.Close();

            if (staleOpenOrders.Count > 0)
            {
                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Auto-closed {Count} stale daily orders", staleOpenOrders.Count);
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
