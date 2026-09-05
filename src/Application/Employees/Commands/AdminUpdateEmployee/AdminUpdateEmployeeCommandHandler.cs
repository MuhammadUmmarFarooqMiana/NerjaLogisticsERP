using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.AdminUpdateEmployee;

public class AdminUpdateEmployeeCommandHandler : IRequestHandler<AdminUpdateEmployeeCommand>
{
    private readonly IApplicationDbContext _context;
    public AdminUpdateEmployeeCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AdminUpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.Id.ToString());

        if (!string.IsNullOrWhiteSpace(request.IqamaNumber))
        {
            var iqamaTaken = await _context.Employees
                .AnyAsync(e => e.IqamaNumber == request.IqamaNumber && e.Id != employee.Id, cancellationToken);
            if (iqamaTaken)
                throw new ConflictException("An employee with this Iqama number already exists.");
        }

        employee.AdminUpdateProfile(
            request.FullName, request.IqamaNumber, request.PlatformId,
            request.IdExpiryDate, request.IqamaExpiryDate,
            request.DrivingLicenseExpiryDate, request.InsuranceExpiryDate);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
