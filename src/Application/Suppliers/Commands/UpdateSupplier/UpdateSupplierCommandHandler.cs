using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Suppliers.Commands.UpdateSupplier;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSupplierCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Supplier), request.Id.ToString());

        supplier.Update(request.Name, request.Email, request.Phone, request.Address);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
