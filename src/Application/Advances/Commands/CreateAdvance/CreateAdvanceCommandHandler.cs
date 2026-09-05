using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Advances.Commands.CreateAdvance;

public class CreateAdvanceCommandHandler : IRequestHandler<CreateAdvanceCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<CreateAdvanceCommandHandler> _logger;

    public CreateAdvanceCommandHandler(IApplicationDbContext context, IUser currentUser, ILogger<CreateAdvanceCommandHandler> logger)
    {
        _context = context; _currentUser = currentUser; _logger = logger;
    }

    public async Task<Guid> Handle(CreateAdvanceCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var advance = Advance.Create(request.EmployeeId, request.Amount, request.AdvanceDate, request.Remarks, _currentUser.Id!.Value);
        _context.Advances.Add(advance);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Advance {Amount} recorded for Employee {EmployeeId}", request.Amount, request.EmployeeId);
        return advance.Id;
    }
}
