using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;

public class SubmitProfileForReviewCommandHandler : IRequestHandler<SubmitProfileForReviewCommand>
{
    private readonly IApplicationDbContext _context;

    public SubmitProfileForReviewCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(SubmitProfileForReviewCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var iqamaTaken = await _context.Employees
            .AnyAsync(e => e.IqamaNumber == request.IqamaNumber && e.Id != request.EmployeeId, cancellationToken);
        if (iqamaTaken)
            throw new ConflictException("An employee with this Iqama number already exists.");

        employee.SubmitProfileForReview(
            request.IqamaNumber, request.IdExpiryDate, request.IqamaExpiryDate,
            request.DrivingLicenseExpiryDate, request.InsuranceExpiryDate);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
