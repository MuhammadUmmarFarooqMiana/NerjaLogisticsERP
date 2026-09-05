using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Expenses.Commands.CreateExpense;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<CreateExpenseCommandHandler> _logger;

    public CreateExpenseCommandHandler(IApplicationDbContext context, IUser currentUser, ILogger<CreateExpenseCommandHandler> logger)
    {
        _context = context; _currentUser = currentUser; _logger = logger;
    }

    public async Task<Guid> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = Expense.Create(request.Category, request.Amount, request.ExpenseDate, request.Description, request.PlatformId, _currentUser.Id!.Value);
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Expense {Amount} ({Category}) recorded", request.Amount, request.Category);
        return expense.Id;
    }
}
