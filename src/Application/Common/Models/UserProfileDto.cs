namespace NerjaLogisticsERP.Application.Common.Models;

public record UserProfileDto
{
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool HasWhatsApp { get; init; }
    public bool EmailConfirmed { get; init; }
}
