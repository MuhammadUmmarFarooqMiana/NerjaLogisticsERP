using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetEmployeeByIdQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<EmployeeDetailDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.Employees
            .Where(e => e.Id == request.Id)
            .Select(e => new
            {
                e.UserId,
                Dto = new EmployeeDetailDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    AccountStatus = e.AccountStatus.ToString(),
                    PerformanceStatus = e.PerformanceStatus.ToString(),
                    IqamaNumber = e.IqamaNumber,
                    PlatformIdNumber = e.PlatformIdNumber,
                    PlatformId = e.PlatformId,
                    PlatformName = e.Platform != null ? e.Platform.Name : null,
                    VehicleId = e.VehicleId,
                    VehicleRegistrationNumber = e.Vehicle != null ? e.Vehicle.RegistrationNumber : null,
                    SupervisorId = e.SupervisorId,
                    SupervisorName = e.Supervisor != null ? e.Supervisor.FullName : null,
                    JoiningDate = e.JoiningDate,
                    IdExpiryDate = e.IdExpiryDate,
                    IqamaExpiryDate = e.IqamaExpiryDate,
                    DrivingLicenseExpiryDate = e.DrivingLicenseExpiryDate,
                    InsuranceExpiryDate = e.InsuranceExpiryDate,
                    ProfileSubmittedAt = e.ProfileSubmittedAt,
                    RejectionReason = e.RejectionReason
                }
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.Id.ToString());

        var profile = await _identityService.GetUserProfileAsync(result.UserId);

        return result.Dto with
        {
            Email = profile?.Email,
            PhoneNumber = profile?.PhoneNumber,
            HasWhatsApp = profile?.HasWhatsApp ?? false,
            EmailConfirmed = profile?.EmailConfirmed ?? false
        };
    }
}
