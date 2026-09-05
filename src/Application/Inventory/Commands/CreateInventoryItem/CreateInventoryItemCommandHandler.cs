using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Inventory.Commands.CreateInventoryItem;

public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateInventoryItemCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.InventoryItems.AnyAsync(i => i.ItemName == request.ItemName, cancellationToken);
        if (exists) throw new ConflictException($"An item named '{request.ItemName}' already exists.");

        var item = InventoryItem.Create(request.ItemName, request.Unit, request.ReorderLevel);
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
