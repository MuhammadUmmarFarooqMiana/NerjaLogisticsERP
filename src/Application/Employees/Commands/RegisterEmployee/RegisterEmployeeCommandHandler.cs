using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.RegisterEmployee;


public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public RegisterEmployeeCommandHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<Guid> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (await _identityService.EmailExistsAsync(request.Email))
            throw new ConflictException("An account with this email already exists.");

        var (result, userId) = await _identityService.CreateUserAsync(
            request.Email, request.Password, request.PhoneNumber, request.HasWhatsApp);

        if (!result.Succeeded)
            throw new ConflictException(string.Join("; ", result.Errors));

        var roleResult = await _identityService.AddToRoleAsync(userId, Roles.Rider);
        if (!roleResult.Succeeded)
            throw new ConflictException(string.Join("; ", roleResult.Errors));

        var employee = Employee.Create(userId, request.FullName);

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
