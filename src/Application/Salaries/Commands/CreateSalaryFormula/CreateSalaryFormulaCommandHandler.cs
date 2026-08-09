using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;

[Authorize(Roles = Roles.Administrator)]
public class CreateSalaryFormulaCommandHandler : IRequestHandler<CreateSalaryFormulaCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<CreateSalaryFormulaCommandHandler> _logger;

    public CreateSalaryFormulaCommandHandler(IApplicationDbContext context, IUser currentUser, ILogger<CreateSalaryFormulaCommandHandler> logger)
    { _context = context; _currentUser = currentUser; _logger = logger; }

    //public async Task<Guid> Handle(CreateSalaryFormulaCommand request, CancellationToken cancellationToken)
    //{
    //    var existing = await _context.SalaryFormulas
    //        .Where(f => f.PlatformId == request.PlatformId && f.EffectiveTo == null)
    //        .ToListAsync(cancellationToken);
    //    // This will close the existing formula before creating new one.
    //    foreach (var old in existing)
    //        old.Close(request.EffectiveFrom.AddDays(-1));

    //    //Or we can restrict user to create new before Deactivate the existing
    //    //if (existing)
    //    //    throw new ConflictException(
    //    //        "An active salary formula already exists for this platform. Deactivate it before creating a new one.");

    //    //var currentUserId = _currentUser.Id
    //    //    ?? throw new UnauthorizedAccessException("No authenticated user id found when creating salary formula.");

    //    var formula = request.FormulaType == SalaryFormulaType.FixedMonthly
    //        ? SalaryFormula.CreateFixedMonthly(request.PlatformId, request.FixedMonthlyAmount!.Value, request.EffectiveFrom, _currentUser.Id!.Value)
    //        : SalaryFormula.CreateTiered(request.PlatformId, request.EffectiveFrom, _currentUser.Id!.Value);

    //    foreach (var t in request.Tiers)
    //        formula.AddTier(t.MinOrders, t.MaxOrders, t.RateType, t.Rate);

    //    _context.SalaryFormulas.Add(formula);
    //    await _context.SaveChangesAsync(cancellationToken);

    //    _logger.LogInformation("Salary formula created for Platform {PlatformId}, effective {Date}", request.PlatformId, request.EffectiveFrom);
    //    return formula.Id;
    //}

    public async Task<Guid> Handle(CreateSalaryFormulaCommand request, CancellationToken cancellationToken)
    {
        var activeExists = await _context.SalaryFormulas
            .AnyAsync(f => f.PlatformId == request.PlatformId && f.EffectiveTo == null, cancellationToken);

        if (activeExists)
            throw new ConflictException(
                "An active salary formula already exists for this platform. Deactivate it before creating a new one.");

        // Prevent overlapping date ranges: the new formula's start date must come
        // strictly after any previously closed formula's end date for this platform.
        var latestEffectiveTo = await _context.SalaryFormulas
            .Where(f => f.PlatformId == request.PlatformId && f.EffectiveTo != null)
            .Select(f => f.EffectiveTo)
            .OrderByDescending(d => d)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestEffectiveTo.HasValue && request.EffectiveFrom <= latestEffectiveTo.Value)
            throw new ConflictException(
                $"EffectiveFrom must be after {latestEffectiveTo:yyyy-MM-dd}, the end date of the most recently closed formula for this platform.");

        var currentUserId = _currentUser.Id
            ?? throw new UnauthorizedAccessException("No authenticated user id found when creating salary formula.");

        var formula = request.FormulaType == SalaryFormulaType.FixedMonthly
            ? SalaryFormula.CreateFixedMonthly(request.PlatformId, request.FixedMonthlyAmount!.Value, request.EffectiveFrom, currentUserId)
            : SalaryFormula.CreateTiered(request.PlatformId, request.EffectiveFrom, currentUserId);

        foreach (var t in request.Tiers)
            formula.AddTier(t.MinOrders, t.MaxOrders, t.RateType, t.Rate);

        _context.SalaryFormulas.Add(formula);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Salary formula created for Platform {PlatformId}, effective {Date}", request.PlatformId, request.EffectiveFrom);
        return formula.Id;
    }
}
