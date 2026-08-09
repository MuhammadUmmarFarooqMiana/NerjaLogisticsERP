using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentById;

public class GetEmployeeDocumentByIdQueryHandler : IRequestHandler<GetEmployeeDocumentByIdQuery, EmployeeDocumentDto>
{
    private readonly IApplicationDbContext _context;
    public GetEmployeeDocumentByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<EmployeeDocumentDto> Handle(GetEmployeeDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var employeeDocuments = await _context.EmployeeDocuments
            .Where(d => d.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        return employeeDocuments ?? throw new NotFoundException(nameof(EmployeeDocument), request.Id.ToString());
    }
}
