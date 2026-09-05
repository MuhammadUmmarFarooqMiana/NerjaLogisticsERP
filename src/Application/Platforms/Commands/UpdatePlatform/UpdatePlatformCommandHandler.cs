using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Platforms.Commands.UpdatePlatform;

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
