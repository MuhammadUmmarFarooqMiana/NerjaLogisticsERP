namespace NerjaLogisticsERP.Application.Platforms.Commands.CreatePlatform;

public record CreatePlatformCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
}
