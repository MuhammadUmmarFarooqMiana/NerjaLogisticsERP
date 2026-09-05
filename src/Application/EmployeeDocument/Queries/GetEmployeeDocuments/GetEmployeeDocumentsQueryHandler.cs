using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocuments;

public class GetEmployeeDocumentsQueryHandler : IRequestHandler<GetEmployeeDocumentsQuery, List<EmployeeDocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    public GetEmployeeDocumentsQueryHandler(IApplicationDbContext context, IUser currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task<List<EmployeeDocumentDto>> Handle(GetEmployeeDocumentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id!.Value;
        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        var isSupervisor = _currentUser.Roles?.Contains(Roles.Supervisor) ?? false;

        var query = _context.EmployeeDocuments.AsQueryable();

        if (!isAdministrator)
        {
            // Plain Supervisor is scoped to their own reports; anyone else (Employee/Rider)
            // can only ever see their own documents, regardless of requested filters.
            query = isSupervisor
                ? query.Where(d => d.Employee.Supervisor != null && d.Employee.Supervisor.UserId == userId)
                : query.Where(d => d.Employee.UserId == userId);
        }

        if ((isAdministrator || isSupervisor) && request.EmployeeId.HasValue)
            query = query.Where(d => d.EmployeeId == request.EmployeeId);

        if ((isAdministrator || isSupervisor) && request.PlatformId.HasValue)
            query = query.Where(d => d.Employee.PlatformId == request.PlatformId);

        return await query
            .OrderByDescending(d => d.Created)
            .Select(d => new EmployeeDocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                EmployeeName = d.Employee.FullName,
                Type = d.Type.ToString(),
                OriginalFileName = d.OriginalFileName,
                ContentType = d.ContentType,
                Created = d.Created
            })
            .ToListAsync(cancellationToken);
    }
}
