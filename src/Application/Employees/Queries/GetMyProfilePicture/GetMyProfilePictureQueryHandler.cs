using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetMyProfilePicture;

public class GetMyProfilePictureQueryHandler : IRequestHandler<GetMyProfilePictureQuery, DocumentFileResult?>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _user;

    public GetMyProfilePictureQueryHandler(IApplicationDbContext context, IFileStorageService storage, IUser user)
    {
        _context = context;
        _storage = storage;
        _user = user;
    }

    public async Task<DocumentFileResult?> Handle(GetMyProfilePictureQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var employee = await _context.Employees
            .Where(e => e.UserId == userId)
            .Select(e => new { e.ProfilePictureStorageKey, e.ProfilePictureContentType })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), userId.ToString());

        // No picture uploaded yet is the default, routine state for most users — not
        // an error, so it's a plain null return rather than a thrown NotFoundException.
        // UnhandledExceptionBehaviour logs every exception that passes through the
        // pipeline as an Error with a full stack trace; throwing here would mean every
        // avatar render for every user without a photo spams the log as if something
        // were actually wrong.
        if (string.IsNullOrEmpty(employee.ProfilePictureStorageKey))
            return null;

        var content = await _storage.GetAsync(employee.ProfilePictureStorageKey, cancellationToken);
        if (content is null)
            return null;

        return new DocumentFileResult(content, employee.ProfilePictureContentType ?? "application/octet-stream", "profile-picture");
    }
}
