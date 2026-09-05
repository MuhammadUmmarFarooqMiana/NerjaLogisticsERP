using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, EmployeeDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IUser _user;

    public GetMyProfileQueryHandler(IApplicationDbContext context, IIdentityService identityService, IUser user)
    {
        _context = context;
        _identityService = identityService;
        _user = user;
    }

    public async Task<EmployeeDetailDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;

        var result = await _context.Employees
            .Where(e => e.UserId == userId)
            .Select(e => new EmployeeDetailDto
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
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), userId.ToString());

        var profile = await _identityService.GetUserProfileAsync(userId);

        return result with
        {
            Email = profile?.Email,
            PhoneNumber = profile?.PhoneNumber,
            HasWhatsApp = profile?.HasWhatsApp ?? false,
            EmailConfirmed = profile?.EmailConfirmed ?? false
        };
    }
}
