using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Platforms.Commands.UpdatePlatform;

[Authorize(Roles = Roles.Administrator)]
public class UpdatePlatformCommandHandler : IRequestHandler<UpdatePlatformCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdatePlatformCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdatePlatformCommand request, CancellationToken cancellationToken)
    {
        var platform = await _context.Platforms.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Platform), request.Id.ToString());

        var nameTaken = await _context.Platforms
            .AnyAsync(p => p.Name == request.Name && p.Id != request.Id, cancellationToken);
        if (nameTaken) throw new ConflictException($"A platform named '{request.Name}' already exists.");

        platform.UpdateName(request.Name);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
