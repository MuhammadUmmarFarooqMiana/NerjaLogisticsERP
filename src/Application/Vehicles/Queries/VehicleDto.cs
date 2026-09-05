namespace NerjaLogisticsERP.Application.Vehicles.Queries;

public record VehicleDto
{
    public Guid Id { get; init; }
    public string RegistrationNumber { get; init; } = default!;
    public string VehicleType { get; init; } = default!;
    public bool IsActive { get; init; }
    public string? AssignedEmployeeName { get; init; }
}
