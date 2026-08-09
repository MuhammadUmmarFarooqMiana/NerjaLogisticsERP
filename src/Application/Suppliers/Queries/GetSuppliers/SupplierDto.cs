namespace NerjaLogisticsERP.Application.Suppliers.Queries.GetSuppliers;

public record SupplierDto(Guid Id, string Name, string? Email, string? Phone, string? Address);
