using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Platforms.Commands.DeletePlatform;

[Authorize(Roles = Roles.Administrator)]
public class DeletePlatformCommandHandler : IRequestHandler<DeletePlatformCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePlatformCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeletePlatformCommand request, CancellationToken cancellationToken)
    {
        var platform = await _context.Platforms.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Platform), request.Id.ToString());

        var inUse = await _context.Employees.AnyAsync(e => e.PlatformId == request.Id, cancellationToken);
        if (inUse)
            throw new ConflictException("Cannot delete a platform that has employees assigned to it.");

        _context.Platforms.Remove(platform);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
