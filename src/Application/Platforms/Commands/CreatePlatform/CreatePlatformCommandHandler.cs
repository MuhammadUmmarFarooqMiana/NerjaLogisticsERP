using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Platforms.Commands.CreatePlatform;

[Authorize(Roles = Roles.Administrator)]
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
