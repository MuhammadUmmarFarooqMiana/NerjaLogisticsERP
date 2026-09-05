using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.CompanyDocuments.Commands.DeleteCompanyDocument;

// Administrator-only — matches folder deletion: a shared/company document can
// have been uploaded by anyone, so removing one is an oversight action.
[Authorize(Roles = Roles.Administrator)]
public record DeleteCompanyDocumentCommand : IRequest
{
    public Guid Id { get; init; }
}
