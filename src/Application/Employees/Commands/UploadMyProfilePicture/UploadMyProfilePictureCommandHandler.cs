using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.UploadMyProfilePicture;

public class UploadMyProfilePictureCommandHandler : IRequestHandler<UploadMyProfilePictureCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _user;

    public UploadMyProfilePictureCommandHandler(IApplicationDbContext context, IFileStorageService storage, IUser user)
    {
        _context = context;
        _storage = storage;
        _user = user;
    }

    public async Task Handle(UploadMyProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), userId.ToString());

        var storageKey = await _storage.SaveAsync(
            request.Content, request.FileName, $"employees/{employee.Id}/profile-picture", cancellationToken);
        var previousStorageKey = employee.SetProfilePicture(storageKey, request.ContentType);

        await _context.SaveChangesAsync(cancellationToken);

        // Best-effort cleanup of the file it replaced — an orphaned old file isn't
        // worth failing the whole upload over.
        if (!string.IsNullOrEmpty(previousStorageKey))
            await _storage.DeleteAsync(previousStorageKey, cancellationToken);
    }
}
