using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.RemoveMyProfilePicture;

public class RemoveMyProfilePictureCommandHandler : IRequestHandler<RemoveMyProfilePictureCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _user;

    public RemoveMyProfilePictureCommandHandler(IApplicationDbContext context, IFileStorageService storage, IUser user)
    {
        _context = context;
        _storage = storage;
        _user = user;
    }

    public async Task Handle(RemoveMyProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), userId.ToString());

        var previousStorageKey = employee.RemoveProfilePicture();
        await _context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(previousStorageKey))
            await _storage.DeleteAsync(previousStorageKey, cancellationToken);
    }
}
