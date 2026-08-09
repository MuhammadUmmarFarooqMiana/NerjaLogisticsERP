namespace NerjaLogisticsERP.Application.Vehicles.Queries;

public record VehicleHistoryDto
{
    public List<ServiceRecordDto> ServiceHistory { get; init; } = new();
    public List<OilChangeRecordDto> OilChanges { get; init; } = new();
    public List<TyreReplacementRecordDto> TyreReplacements { get; init; } = new();
    public List<AccidentRecordDto> AccidentHistory { get; init; } = new();
}

public record ServiceRecordDto(Guid Id, DateOnly ServiceDate, int Odometer, string Description, decimal Cost);
public record OilChangeRecordDto(Guid Id, DateOnly ChangeDate, int Odometer, decimal Cost);
public record TyreReplacementRecordDto(Guid Id, DateOnly ReplacementDate, int Odometer, int NumberOfTyres, decimal Cost);
public record AccidentRecordDto(Guid Id, DateOnly AccidentDate, string Description, decimal RepairCost);
