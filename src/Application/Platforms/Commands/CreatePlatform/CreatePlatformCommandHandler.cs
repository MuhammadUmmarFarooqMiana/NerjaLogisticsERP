using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Platforms.Commands.CreatePlatform;

public class CreatePlatformCommandHandler : IRequestHandler<CreatePlatformCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreatePlatformCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreatePlatformCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Platforms.AnyAsync(p => p.Name == request.Name, cancellationToken);
        if (exists) throw new ConflictException($"A platform named '{request.Name}' already exists.");

        var platform = Platform.Create(request.Name);
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync(cancellationToken);

        return platform.Id;
    }
}
