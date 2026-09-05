namespace NerjaLogisticsERP.Application.Reports.PlatformReconciliation;

public class PlatformReconciliationReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string PlatformName { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;

    /// <summary>Rider present in both the platform sheet and a Nerja MonthlySummary for the period.</summary>
    public List<PlatformReconciliationRowDto> MatchedRows { get; set; } = [];

    /// <summary>Rider in the platform sheet whose rider_id matches no Employee.PlatformIdNumber
    /// on this platform (or that employee has no MonthlySummary for the period yet) — the
    /// platform is paying for someone Nerja has no record to compare against.</summary>
    public List<PlatformReconciliationUnmatchedPlatformRowDto> UnmatchedPlatformRows { get; set; } = [];

    /// <summary>Employee on this platform with a MonthlySummary for the period, but absent from
    /// the uploaded sheet entirely — Nerja expects payment for someone the platform's file
    /// doesn't account for.</summary>
    public List<PlatformReconciliationMissingFromSheetDto> MissingFromSheetRows { get; set; } = [];

    public int TotalMatched { get; set; }
    public int TotalUnmatchedInPlatform { get; set; }
    public int TotalMissingFromSheet { get; set; }
    public int TotalOrdersMismatchCount { get; set; }
    public decimal TotalPenalties { get; set; }
    public decimal TotalOriginalNetPayable { get; set; }
    public decimal TotalAdjustedNetPayable { get; set; }
}

public class PlatformReconciliationRowDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string PlatformIdNumber { get; set; } = default!;
    public int NerjaCompletedOrders { get; set; }
    public int PlatformCompletedOrders { get; set; }
    /// <summary>Platform − Nerja. Zero means the two sides agree.</summary>
    public int OrdersDifference { get; set; }

    public decimal StackingDeduction { get; set; }
    public decimal DeclinedPenaltiesDayLogic { get; set; }
    public decimal LatePenalty { get; set; }
    public decimal NoShowPenalty { get; set; }
    public decimal NoShowPenaltySpecialCities { get; set; }
    public decimal DailyAcceptanceRatePenalty { get; set; }
    public decimal MissedDaysPenalty { get; set; }
    /// <summary>Sum of the seven penalty columns above, exactly as the platform reported them
    /// (already negative-or-zero in the source file — this is not re-signed).</summary>
    public decimal TotalPenalties { get; set; }

    /// <summary>Employee.MonthlySummary net payable before this reconciliation's adjustment.</summary>
    public decimal OriginalNetPayable { get; set; }
    /// <summary>OriginalNetPayable + TotalPenalties (adding, since penalties are already negative).</summary>
    public decimal AdjustedNetPayable { get; set; }
    public string MonthlySummaryStatus { get; set; } = default!;
}

public class PlatformReconciliationUnmatchedPlatformRowDto
{
    public string PlatformIdNumber { get; set; } = default!;
    public int PlatformCompletedOrders { get; set; }
    public decimal TotalPenalties { get; set; }
}

public class PlatformReconciliationMissingFromSheetDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string? PlatformIdNumber { get; set; }
    public int NerjaCompletedOrders { get; set; }
    public decimal NetPayable { get; set; }
}
