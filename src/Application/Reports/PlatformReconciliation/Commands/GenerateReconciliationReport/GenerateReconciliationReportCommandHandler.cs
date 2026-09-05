using System.Globalization;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.GenerateReconciliationReport;

public class GenerateReconciliationReportCommandHandler : IRequestHandler<GenerateReconciliationReportCommand, PlatformReconciliationReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPlatformReconciliationFileParser _parser;

    public GenerateReconciliationReportCommandHandler(IApplicationDbContext context, IPlatformReconciliationFileParser parser)
    {
        _context = context;
        _parser = parser;
    }

    public async Task<PlatformReconciliationReportDto> Handle(GenerateReconciliationReportCommand request, CancellationToken cancellationToken)
    {
        var platform = await _context.Platforms.FindAsync(new object[] { request.PlatformId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Platform), request.PlatformId.ToString());

        var platformRows = _parser.ParseRiderLevelSheet(request.Content);

        // Nerja side of the comparison: employees on this platform who already have a
        // MonthlySummary for the period — without one there's no NerjaCompletedOrders/
        // NetPayable to compare against, so those employees can only ever show up in
        // MissingFromSheetRows if we scoped wider, which would just be noise here.
        var summaries = await _context.MonthlySummaries
            .Include(s => s.Employee)
            .Where(s => s.Year == request.Year && s.Month == request.Month && s.Employee.PlatformId == request.PlatformId)
            .ToListAsync(cancellationToken);

        var summaryByPlatformId = summaries
            .Where(s => !string.IsNullOrWhiteSpace(s.Employee.PlatformIdNumber))
            .ToDictionary(s => s.Employee.PlatformIdNumber!.Trim(), s => s, StringComparer.OrdinalIgnoreCase);

        var matchedRows = new List<PlatformReconciliationRowDto>();
        var unmatchedPlatformRows = new List<PlatformReconciliationUnmatchedPlatformRowDto>();
        var matchedPlatformIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in platformRows)
        {
            var penaltiesSum = row.StackingDeduction + row.DeclinedPenaltiesDayLogic + row.LatePenalty
                + row.NoShowPenalty + row.NoShowPenaltySpecialCities + row.DailyAcceptanceRatePenalty + row.MissedDaysPenalty;

            if (summaryByPlatformId.TryGetValue(row.RiderId, out var summary))
            {
                matchedPlatformIds.Add(row.RiderId);
                matchedRows.Add(new PlatformReconciliationRowDto
                {
                    EmployeeId = summary.EmployeeId,
                    EmployeeName = summary.Employee.FullName,
                    PlatformIdNumber = row.RiderId,
                    NerjaCompletedOrders = summary.TotalCompletedOrders,
                    PlatformCompletedOrders = row.CompletedOrders,
                    OrdersDifference = row.CompletedOrders - summary.TotalCompletedOrders,
                    StackingDeduction = row.StackingDeduction,
                    DeclinedPenaltiesDayLogic = row.DeclinedPenaltiesDayLogic,
                    LatePenalty = row.LatePenalty,
                    NoShowPenalty = row.NoShowPenalty,
                    NoShowPenaltySpecialCities = row.NoShowPenaltySpecialCities,
                    DailyAcceptanceRatePenalty = row.DailyAcceptanceRatePenalty,
                    MissedDaysPenalty = row.MissedDaysPenalty,
                    TotalPenalties = penaltiesSum,
                    OriginalNetPayable = summary.NetSalaryPayable,
                    AdjustedNetPayable = summary.NetSalaryPayable + penaltiesSum,
                    MonthlySummaryStatus = summary.Status.ToString()
                });
            }
            else
            {
                unmatchedPlatformRows.Add(new PlatformReconciliationUnmatchedPlatformRowDto
                {
                    PlatformIdNumber = row.RiderId,
                    PlatformCompletedOrders = row.CompletedOrders,
                    TotalPenalties = penaltiesSum
                });
            }
        }

        var missingFromSheetRows = summaries
            .Where(s => string.IsNullOrWhiteSpace(s.Employee.PlatformIdNumber)
                || !matchedPlatformIds.Contains(s.Employee.PlatformIdNumber!.Trim()))
            .Select(s => new PlatformReconciliationMissingFromSheetDto
            {
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FullName,
                PlatformIdNumber = s.Employee.PlatformIdNumber,
                NerjaCompletedOrders = s.TotalCompletedOrders,
                NetPayable = s.NetSalaryPayable
            })
            .OrderBy(x => x.EmployeeName)
            .ToList();

        matchedRows = matchedRows.OrderBy(r => r.EmployeeName).ToList();
        unmatchedPlatformRows = unmatchedPlatformRows.OrderBy(r => r.PlatformIdNumber).ToList();

        return new PlatformReconciliationReportDto
        {
            Year = request.Year,
            Month = request.Month,
            PlatformName = platform.Name,
            PeriodLabel = new DateOnly(request.Year, request.Month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture),
            MatchedRows = matchedRows,
            UnmatchedPlatformRows = unmatchedPlatformRows,
            MissingFromSheetRows = missingFromSheetRows,
            TotalMatched = matchedRows.Count,
            TotalUnmatchedInPlatform = unmatchedPlatformRows.Count,
            TotalMissingFromSheet = missingFromSheetRows.Count,
            TotalOrdersMismatchCount = matchedRows.Count(r => r.OrdersDifference != 0),
            TotalPenalties = matchedRows.Sum(r => r.TotalPenalties),
            TotalOriginalNetPayable = matchedRows.Sum(r => r.OriginalNetPayable),
            TotalAdjustedNetPayable = matchedRows.Sum(r => r.AdjustedNetPayable)
        };
    }
}
