namespace NerjaLogisticsERP.Application.Reports.Vehicles;

public class VehiclesReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public decimal TotalMaintenanceCost { get; set; }

    public List<VehiclesReportRowDto> Rows { get; set; } = [];
}

public class VehiclesReportRowDto
{
    public string VehicleRegistration { get; set; } = default!;
    /// <summary>Allocation, Return, Service, OilChange, TyreReplacement, or Accident.</summary>
    public string RecordType { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
}
