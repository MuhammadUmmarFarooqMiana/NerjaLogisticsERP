using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.EmployeeDocument.Commands.UploadEmployeeDocument;

public class UploadEmployeeDocumentCommandHandler : IRequestHandler<UploadEmployeeDocumentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IUser _currentUser;
    private readonly ILogger<UploadEmployeeDocumentCommandHandler> _logger;

    public UploadEmployeeDocumentCommandHandler(IApplicationDbContext context, IFileStorageService storage, IUser currentUser, ILogger<UploadEmployeeDocumentCommandHandler> logger)
    { _context = context; _storage = storage; _currentUser = currentUser; _logger = logger; }

    public async Task<Guid> Handle(UploadEmployeeDocumentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id!.Value;
        var isAdministrator = _currentUser.Roles?.Contains(Roles.Administrator) ?? false;
        var isSupervisor = _currentUser.Roles?.Contains(Roles.Supervisor) ?? false;

        Guid employeeId;
        if ((isAdministrator || isSupervisor) && request.EmployeeId.HasValue)
        {
            employeeId = request.EmployeeId.Value;
            if (!isAdministrator)
            {
                var isOwnReport = await _context.Employees.AnyAsync(
                    e => e.Id == employeeId && e.Supervisor != null && e.Supervisor.UserId == userId,
                    cancellationToken);
                if (!isOwnReport) throw new ForbiddenAccessException();
            }
        }
        else
        {
            employeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var exists = await _context.Employees.AnyAsync(e => e.Id == employeeId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Employee), employeeId.ToString());

        var storageKey = await _storage.SaveAsync(request.Content, request.FileName, $"employees/{employeeId}", cancellationToken);

        var doc = Domain.Entities.EmployeeDocument.Create(employeeId, request.Type, storageKey, request.FileName, request.ContentType, userId);
        _context.EmployeeDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document {Type} uploaded for Employee {EmployeeId}", request.Type, employeeId);
        return doc.Id;
    }
}
