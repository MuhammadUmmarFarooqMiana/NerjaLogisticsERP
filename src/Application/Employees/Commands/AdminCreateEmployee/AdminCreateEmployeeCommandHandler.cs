using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.AdminCreateEmployee;

public class AdminCreateEmployeeCommandHandler : IRequestHandler<AdminCreateEmployeeCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public AdminCreateEmployeeCommandHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<Guid> Handle(AdminCreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (await _identityService.EmailExistsAsync(request.Email))
            throw new ConflictException("An account with this email already exists.");

        var iqamaTaken = await _context.Employees.AnyAsync(e => e.IqamaNumber == request.IqamaNumber, cancellationToken);
        if (iqamaTaken)
            throw new ConflictException("An employee with this Iqama number already exists.");

        var (result, userId) = await _identityService.CreateUserAsync(
            request.Email, request.Password, request.PhoneNumber, request.HasWhatsApp);
        if (!result.Succeeded)
            throw new ConflictException(string.Join("; ", result.Errors));

        foreach (var role in request.Roles)
        {
            var roleResult = await _identityService.AddToRoleAsync(userId, role);
            if (!roleResult.Succeeded)
                throw new ConflictException(string.Join("; ", roleResult.Errors));
        }

        var employee = Employee.Create(userId, request.FullName);

        employee.SubmitProfileForReview(
            request.IqamaNumber, request.PlatformIdNumber, request.IdExpiryDate, request.IqamaExpiryDate,
            request.DrivingLicenseExpiryDate, request.InsuranceExpiryDate);

        if (request.PlatformId.HasValue)
            employee.AssignPlatform(request.PlatformId.Value);

        employee.Approve(request.JoiningDate);

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
