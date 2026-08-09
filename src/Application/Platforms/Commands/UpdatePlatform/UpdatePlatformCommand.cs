namespace NerjaLogisticsERP.Application.Platforms.Commands.UpdatePlatform;

public record UpdatePlatformCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
