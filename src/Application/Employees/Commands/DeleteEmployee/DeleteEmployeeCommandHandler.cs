using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public DeleteEmployeeCommandHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.Id.ToString());

        // Soft delete: the global query filter on IsDeleted hides them from every list/detail
        // query from here on, and LoginCommandHandler's employee-lookup uses the same filtered
        // set, so a deleted employee's account stops being usable to log in too — without
        // touching any Advances/Fines/DailyOrders/MonthlySummaries history that references them.
        employee.Delete(_currentUser.Id);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
