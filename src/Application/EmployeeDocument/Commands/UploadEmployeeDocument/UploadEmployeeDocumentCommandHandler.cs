using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
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
        var exists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var storageKey = await _storage.SaveAsync(request.Content, request.FileName, $"employees/{request.EmployeeId}", cancellationToken);

        var doc = Domain.Entities.EmployeeDocument.Create(request.EmployeeId, request.Type, storageKey, request.FileName, request.ContentType, _currentUser.Id!.Value);
        _context.EmployeeDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document {Type} uploaded for Employee {EmployeeId}", request.Type, request.EmployeeId);
        return doc.Id;
    }
}
