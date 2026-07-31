using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeProfileCommandHandler : IRequestHandler<UpdateEmployeeProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public UpdateEmployeeProfileCommandHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateEmployeeProfileCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == _currentUser.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), _currentUser.Id?.ToString() ?? "unknown");

        var iqamaTaken = await _context.Employees
            .AnyAsync(e => e.IqamaNumber == request.IqamaNumber && e.Id != employee.Id, cancellationToken);
        if (iqamaTaken)
            throw new ConflictException("An employee with this Iqama number already exists.");

        employee.UpdateProfile(
            request.IqamaNumber, request.PlatformId,
            request.IdExpiryDate, request.IqamaExpiryDate,
            request.DrivingLicenseExpiryDate, request.InsuranceExpiryDate);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
