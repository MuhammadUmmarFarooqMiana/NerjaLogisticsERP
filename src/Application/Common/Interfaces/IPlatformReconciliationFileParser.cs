namespace NerjaLogisticsERP.Application.Common.Interfaces;

/// <summary>
/// One rider's row from a platform-provided payout sheet (Keeta/Hunger/Jahez), already
/// normalized to plain numbers — column order/position in the source file doesn't matter,
/// only the "<arabic label> - <english_key>" suffix on each header does (see the parser's
/// own implementation for how that's read).
/// </summary>
public record PlatformRiderRow
{
    public string RiderId { get; init; } = default!;
    public int CompletedOrders { get; init; }
    public decimal StackingDeduction { get; init; }
    public decimal DeclinedPenaltiesDayLogic { get; init; }
    public decimal LatePenalty { get; init; }
    public decimal NoShowPenalty { get; init; }
    public decimal NoShowPenaltySpecialCities { get; init; }
    public decimal DailyAcceptanceRatePenalty { get; init; }
    public decimal MissedDaysPenalty { get; init; }
}

/// <summary>
/// Reads the "Rider LVL" sheet out of a platform-provided payout workbook. Implemented in
/// Infrastructure (needs a real spreadsheet library) — Application only depends on this
/// abstraction so the command handler stays unit-testable and storage/format-agnostic.
/// </summary>
public interface IPlatformReconciliationFileParser
{
    /// <exception cref="Exceptions.ValidationException">
    /// The file isn't a readable workbook, has no "Rider LVL" sheet, or that sheet is
    /// missing one of the columns this report depends on.
    /// </exception>
    List<PlatformRiderRow> ParseRiderLevelSheet(byte[] content);
}
