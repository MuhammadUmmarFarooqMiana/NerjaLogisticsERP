using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Fines.Commands.CreateFine;

public class CreateFineCommandHandler : IRequestHandler<CreateFineCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<CreateFineCommandHandler> _logger;

    public CreateFineCommandHandler(IApplicationDbContext context, IUser currentUser, ILogger<CreateFineCommandHandler> logger)
    {
        _context = context; _currentUser = currentUser; _logger = logger;
    }

    public async Task<Guid> Handle(CreateFineCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var fine = Fine.Create(request.EmployeeId, request.Amount, request.Reason, request.FineDate, _currentUser.Id!.Value);
        _context.Fines.Add(fine);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fine {Amount} recorded for Employee {EmployeeId}: {Reason}", request.Amount, request.EmployeeId, request.Reason);
        return fine.Id;
    }
}
