using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Suppliers.Queries.GetSuppliers;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto>
{
    private readonly IApplicationDbContext _context;

    public GetSupplierByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SupplierDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .Where(s => s.Id == request.Id)
            .Select(s => new SupplierDto(s.Id, s.Name, s.Email, s.Phone, s.Address))
            .FirstOrDefaultAsync(cancellationToken);

        return supplier ?? throw new NotFoundException(nameof(Supplier), request.Id.ToString());
    }
}
