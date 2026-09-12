using FluentValidation.Results;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;

public class SubmitProfileForReviewCommandHandler : IRequestHandler<SubmitProfileForReviewCommand>
{
    // Enforced server-side (not just in the UI) because documents/the profile picture are
    // uploaded through separate endpoints, independent of this command — a client that skips
    // an upload step, or calls the API directly, must still be blocked. The profile picture
    // isn't in this list: it isn't an EmployeeDocument at all, it's Employee.
    // ProfilePictureStorageKey, checked separately below.
    private static readonly EmployeeDocumentType[] RequiredDocumentTypes =
    [
        EmployeeDocumentType.PlatformIdProof
    ];

    private readonly IApplicationDbContext _context;

    public SubmitProfileForReviewCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(SubmitProfileForReviewCommand request, CancellationToken cancellationToken)
    {
        // FindAsync looks up by primary key (Employee.Id) — request.UserId is
        // the FK to ApplicationUser, a different value, so this must be a
        // FirstOrDefaultAsync filter instead (matches UpdateEmployeeProfileCommandHandler's
        // already-correct pattern for the same self-service lookup).
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.UserId.ToString());

        var iqamaTaken = await _context.Employees
            .AnyAsync(e => e.IqamaNumber == request.IqamaNumber && e.Id != employee.Id, cancellationToken);
        if (iqamaTaken)
            throw new ConflictException("An employee with this Iqama number already exists.");

        var uploadedTypes = await _context.EmployeeDocuments
            .Where(d => d.EmployeeId == employee.Id)
            .Select(d => d.Type)
            .ToListAsync(cancellationToken);

        var missingTypes = RequiredDocumentTypes.Where(t => !uploadedTypes.Contains(t)).ToList();
        var failures = missingTypes.Select(t =>
            new ValidationFailure(t.ToString(), $"{t} document must be uploaded before submitting for review.")).ToList();

        if (string.IsNullOrEmpty(employee.ProfilePictureStorageKey))
            failures.Add(new ValidationFailure(nameof(Employee.ProfilePictureStorageKey), "Profile picture must be uploaded before submitting for review."));

        if (failures.Count > 0)
            throw new Application.Common.Exceptions.ValidationException(failures);

        employee.SubmitProfileForReview(
            request.IqamaNumber, request.PlatformIdNumber, request.IdExpiryDate, request.IqamaExpiryDate,
            request.DrivingLicenseExpiryDate, request.InsuranceExpiryDate);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
