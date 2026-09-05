using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentById;

public class GetEmployeeDocumentByIdQueryHandler : IRequestHandler<GetEmployeeDocumentByIdQuery, EmployeeDocumentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    public GetEmployeeDocumentByIdQueryHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task<EmployeeDocumentDto> Handle(GetEmployeeDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.EmployeeDocuments
            .Where(d => d.Id == request.Id)
            .Select(d => new
            {
                Dto = new EmployeeDocumentDto
                {
                    Id = d.Id,
                    EmployeeId = d.EmployeeId,
                    EmployeeName = d.Employee.FullName,
                    Type = d.Type.ToString(),
                    OriginalFileName = d.OriginalFileName,
                    ContentType = d.ContentType,
                    Created = d.Created
                },
                OwnerUserId = d.Employee.UserId,
                SupervisorUserId = d.Employee.Supervisor != null ? d.Employee.Supervisor.UserId : (Guid?)null
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeDocument), request.Id.ToString());

        EnsureCanAccess(result.OwnerUserId, result.SupervisorUserId);
        return result.Dto;
    }

    private void EnsureCanAccess(Guid ownerUserId, Guid? supervisorUserId)
    {
        var userId = _currentUser.Id!.Value;
        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        if (isAdministrator || ownerUserId == userId || supervisorUserId == userId) return;
        throw new ForbiddenAccessException();
    }
}
