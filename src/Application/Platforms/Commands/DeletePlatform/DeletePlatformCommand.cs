namespace NerjaLogisticsERP.Application.Platforms.Commands.DeletePlatform;

public record DeletePlatformCommand : IRequest
{
    public Guid Id { get; init; }
}
