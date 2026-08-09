using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocuments;

public class GetEmployeeDocumentsQueryHandler : IRequestHandler<GetEmployeeDocumentsQuery, List<EmployeeDocumentDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEmployeeDocumentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<EmployeeDocumentDto>> Handle(GetEmployeeDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EmployeeDocuments.AsQueryable();

        if (request.EmployeeId.HasValue)
            query = query.Where(d => d.EmployeeId == request.EmployeeId);

        if (request.PlatformId.HasValue)
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
