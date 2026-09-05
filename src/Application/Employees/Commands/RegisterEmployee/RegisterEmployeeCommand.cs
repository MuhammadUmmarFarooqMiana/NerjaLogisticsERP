namespace NerjaLogisticsERP.Application.Employees.Commands.RegisterEmployee;

public record RegisterEmployeeCommand : IRequest<Guid>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool HasWhatsApp { get; init; } = true;
}
