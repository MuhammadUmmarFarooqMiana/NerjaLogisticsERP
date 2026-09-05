namespace NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanics;

public record MechanicDto(Guid Id, string Name, string? Phone, string? Email, string? Specialty, string? Address);
