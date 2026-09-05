using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Suppliers.Queries.GetSuppliers;

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, PaginatedList<SupplierDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSuppliersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
        => await _context.Suppliers
            .OrderBy(s => s.Name)
            .Select(s => new SupplierDto(s.Id, s.Name, s.Email, s.Phone, s.Address))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
}
